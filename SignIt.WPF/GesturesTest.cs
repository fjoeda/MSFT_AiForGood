using Microsoft.Gestures;
using Microsoft.Gestures.Endpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignIt.WPF
{
    public delegate void GestureChangedHandler(string gestureName);
    public class GesturesTest : IDisposable
    {
        private GesturesServiceEndpoint _gesturesService;
        private Gesture _detectedGesture;

        public event StatusChangedHandler GesturesDetectionStatusChanged;
        public event GestureChangedHandler GestureChanged;

        private Gesture _likeGesture;

        public async Task Init()
        {
            // Set the gesture service
            _gesturesService = GesturesServiceEndpointFactory.Create("localhost");
            _gesturesService.StatusChanged += (oldVal, newVal) => GesturesDetectionStatusChanged?.Invoke(oldVal, newVal);
            await _gesturesService.ConnectAsync();


            var hurufI = new HandPose(
                "Huruf_I",
                new PalmPose(Hand.RightHand, PoseDirection.Up | PoseDirection.Forward),
                new FingerPose(new[] { Finger.Pinky }, FingerFlexion.Open),
                new FingerPose(new[] { Finger.Index, Finger.Ring, Finger.Middle }, FingerFlexion.Folded));
            hurufI.Triggered += (s, arg) => GestureChanged?.Invoke(arg.GestureSegment.Name);

            var piece = new HandPose(
                "Piece",
                new PalmPose(Hand.RightHand, PoseDirection.Up | PoseDirection.Forward),
                new FingerPose(new[] { Finger.Index, Finger.Middle }, FingerFlexion.Open),
                new FingerPose(new[] { Finger.Pinky, Finger.Ring, }, FingerFlexion.Folded));
            piece.Triggered += (s, arg) => GestureChanged?.Invoke(arg.GestureSegment.Name);

            var baik = new HandPose(
                "baik",
                new PalmPose(Hand.RightHand, PoseDirection.Left | PoseDirection.Forward),
                new FingerPose(new[] { Finger.Ring, Finger.Pinky, Finger.Middle }, FingerFlexion.OpenStretched),
                new FingerPose(new[] { Finger.Index, Finger.Thumb, }, FingerFlexion.Folded));
            baik.Triggered += (s, arg) => GestureChanged?.Invoke(arg.GestureSegment.Name);


            var alphabetGesture = new PassThroughGestureSegment("performing_state");

            _detectedGesture = new Gesture("Sample_Gesture", alphabetGesture);
            _detectedGesture.AddSubPath(alphabetGesture, hurufI, alphabetGesture);
            _detectedGesture.AddSubPath(alphabetGesture, piece, alphabetGesture);
            _detectedGesture.AddSubPath(alphabetGesture, baik, alphabetGesture);


            await _gesturesService.RegisterGesture(_detectedGesture, isGlobal: true);
            await RegisterLikeGesture();
            await RegisterThankYouGesture();
        }

        private async Task RegisterLikeGesture()
        {
            // Our starting pose is a fist 
            var fist = new HandPose("Fist", new PalmPose(new AnyHandContext(), PoseDirection.Left | PoseDirection.Right),
                                            new FingerPose(new AllFingersContext(), FingerFlexion.Folded));
            // In the final pose the thumb flexion will open in the "up" direction
            var like = new HandPose("Like", new PalmPose(new AnyHandContext(), PoseDirection.Left | PoseDirection.Right),
                                            new FingerPose(new[] { Finger.Index, Finger.Middle, Finger.Ring, Finger.Pinky }, FingerFlexion.Folded),
                                            new FingerPose(Finger.Thumb, FingerFlexion.Open, PoseDirection.Up));

            // ... finally define the gesture using the hand pose objects defined above forming a simple state machine: fist -> Like
            _likeGesture = new Gesture("LikeGesture", fist, like);
            _likeGesture.Triggered += (s, arg) => GestureChanged?.Invoke(arg.GestureSegment.Name);

            // Registering the like gesture _globally_ (i.e. isGlobal:true), by global registration we mean this gesture will be 
            // detected even it was initiated not by this application or if the this application isn't in focus
            await _gesturesService.RegisterGesture(_likeGesture, isGlobal: true);
        }

        private async Task RegisterThankYouGesture()
        {
            //var forw = new PalmMotion("forward", MotionPlane.Depth);
            var thankyou1 = new HandPose(
                "thankyou1",
                new PalmPose(Hand.RightHand, PoseDirection.Up | PoseDirection.Backward),
                new FingerPose(new[] { Finger.Ring, Finger.Pinky, Finger.Middle, Finger.Index, Finger.Thumb }, FingerFlexion.Open),
                new FingertipDistanceRelation(Finger.Index, RelativeDistance.Touching, Finger.Middle),
                new FingertipDistanceRelation(Finger.Thumb, RelativeDistance.Touching, Finger.Index));

            var thxGesture = new Gesture("thxgesture", thankyou1);
            thxGesture.Triggered += (s, arg) => GestureChanged?.Invoke(arg.GestureSegment.Name);
            await _gesturesService.RegisterGesture(thxGesture, isGlobal: true);
        }

        public void Dispose() => _gesturesService.Dispose();
    }
}
