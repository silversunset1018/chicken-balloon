using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fresvii.AppSteroid.Gui{

    public class FASSwipeInfo
    {
        public enum Direction { Up, Down, Left, Right }

        public Vector2 startPosition;
        public Vector2 endPosition;
        public Direction direction = Direction.Left;

        public FASSwipeInfo(Vector2 startPosition, Vector2 endPosition, Direction direction)
        {
            this.startPosition = startPosition;

            this.endPosition = endPosition;

            this.direction = direction;
        }
    }

    public class FASPinchInfo
    {
        public Vector2 startPosition1;
        public Vector2 position1;

        public Vector2 startPosition2;
        public Vector2 position2;
    }

	public class FASGesture : MonoBehaviour {

        public enum DragDirection { Horizontal, Vertical }

		public static bool IsDragging {get; protected set;}
		public static bool Inertia {get; protected set;}
        public static bool IsInertiaEnd { get; protected set; }
        public static Vector2 Delta { get; protected set; }
		public static Vector2 DragStartPosition{get; protected set;}
		public static Vector2 DragPosition;
		public static Vector2 DragEndPosition;
        public static bool IsHolding { get; protected set; }
        public static bool IsTouchBegin { get; protected set; }
        public static bool IsTouching { get; protected set; }
        public static bool IsTouchEnd { get; protected set; }
        public static DragDirection DragDirec { get; protected set; }
        public static bool IsPinching { get; protected set; }

		private float speed;
		private Vector2 lastVec;
        private Vector2 lastDelta;

        public int guiDepth = -200;

        public float resistance = 0.5f;
        public float epsilon = 0.1f;

        public float dragDetectDistanceInch = 0.2f;
		private float dragDetectDistance = 20;

        public float holdingDetectTime = 1.0f;

		public static FASGesture Instance;

        private float holdStartTime;

		public float stopSpeed = 1.0f;

        private bool isStay;

        private bool pause;

        private Vector2 stayStartPosition;

#if UNITY_EDITOR
        private float stayMagnitude = 10f;
#endif
        private bool postIsDragging;

        private Vector2 postMousePosition;

        public static Vector2 TouchPosition { get; protected set; }

        private Vector2 postTouchPosition;

        public float swipeSpeed = 1500f;

        private Vector2 touchStartPosition = Vector2.zero;

        private static FASPinchInfo pinchInfo = new FASPinchInfo();

        public static event Action<FASSwipeInfo> OnSwipe;

        public static event Action<FASPinchInfo> OnPinch;

        public static event Action<FASPinchInfo> OnPinchStart;

        private static float pinchEndTime;

        private static bool dragBlock = false;

        public int speedsBufFrameCount = 5;

        private List<float> speeds = new List<float>();

        private List<Vector2> vecs = new List<Vector2>();

        private float maxSpeed = 100000f;

		void Awake(){

			if(Instance == null)
            {
				Instance = this;
			}
			else{
				Destroy(this);
			}

            if (Screen.dpi != 0.0f)
            {
                dragDetectDistance = Screen.dpi * dragDetectDistanceInch;
            }
            else
            {
                dragDetectDistance = 400.0f * dragDetectDistanceInch;
            }

            Input.ResetInputAxes();
 		}
				

		private bool brake = false;

        private float inertiaTime;

        IEnumerator DragBlockDownDelay(float interval)
        {
            yield return new WaitForSeconds(interval);

            DragBlockDown();
        }

		void Update(){

			//IsDragging = false;

            IsTouchBegin = false;

            IsTouchEnd = false;
            
            IsTouching = false;
            
            IsInertiaEnd = false;

            Delta = Vector2.zero;

            if (pause)
            {
				lastDelta = Vector2.zero;

                speed = 0.0f;
				
                Inertia = false;
				                
                return;
            }

            if (OnPinch != null)
            {
                if (Input.touchCount > 1)
                {
                    pinchInfo.position1 = Input.touches[0].position;

                    pinchInfo.position2 = Input.touches[1].position;

                    if (!IsPinching)
                    {
                        pinchInfo.startPosition1 = Input.touches[0].position;

                        pinchInfo.startPosition2 = Input.touches[1].position;

                        OnPinchStart(pinchInfo);
                    }

                    IsPinching = true;

                    dragBlock = true;

                    OnPinch(pinchInfo);
                }
                else
                {
                    IsPinching = false;

                    StartCoroutine(DragBlockDownDelay(0.5f));
                }
            }        
            
            if (Input.touchCount > 0 && !IsPinching && !dragBlock)
            {
                IsTouching = true;

                Touch touch = Input.touches[0];

                if (float.IsNaN(touch.position.x) || float.IsInfinity(touch.position.x) || float.IsNegativeInfinity(touch.position.x) || float.IsPositiveInfinity(touch.position.x) ||
                    float.IsNaN(touch.position.y) || float.IsInfinity(touch.position.y) || float.IsNegativeInfinity(touch.position.y) || float.IsPositiveInfinity(touch.position.y))
                {
                    IsTouching = false;

                    Input.ResetInputAxes();

                    return;
                }
                else
                {
                    TouchPosition = touch.position;
                }

                lastDelta = TouchPosition - postTouchPosition;

                if (touch.phase == TouchPhase.Began)
                {
                    IsTouchBegin = true;

                    Inertia = false;

                    DragPosition = DragStartPosition = TouchPosition;

                    holdStartTime = Time.time;

                    postTouchPosition = TouchPosition;

                    lastDelta = Vector2.zero;

                    speed = 0.0f;

                    speeds.Clear();

                    vecs.Clear();
				}				
                else if (touch.phase == TouchPhase.Moved && Vector2.Distance(touch.position, DragStartPosition) > dragDetectDistance)
                {
                    IsDragging = true;

                    float h = Mathf.Abs(touch.position.x - DragStartPosition.x);

                    float v = Mathf.Abs(touch.position.y - DragStartPosition.y);

                    DragDirec = (h > v) ? DragDirection.Horizontal : DragDirection.Vertical;

                    Delta = TouchPosition - postTouchPosition;

                    DragPosition = TouchPosition;

                    speed = touch.deltaPosition.magnitude / touch.deltaTime;

                    speed = Mathf.Min(maxSpeed, speed);

                    speeds.Insert(0, speed);

                    vecs.Insert(0, lastDelta);

                    if (speeds.Count > speedsBufFrameCount)
                    {
                        speeds.RemoveAt(speedsBufFrameCount);

                        vecs.RemoveAt(speedsBufFrameCount);
                    }
                }
                else if (IsDragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
                {
                    IsDragging = false;

                    speed = lastDelta.magnitude / Time.deltaTime;

                    float avSpeed = speed;

                    float totalWeight = 1.0f;

                    int i = 0;

                    foreach (float fSpeed in speeds)
                    {
                        float weight = 1.0f / (i + 2);

                        avSpeed += weight * fSpeed;

                        totalWeight += weight;

                        i++;
                    }

                    speed = avSpeed / totalWeight;

                    speed = Mathf.Min(maxSpeed, speed);

                    lastVec = lastDelta.normalized;

                    if (lastVec == Vector2.zero)
                    {
                        foreach (Vector2 vec in vecs)
                        {
                            if (vec != Vector2.zero)
                            {
                                lastVec = vec.normalized;

                                break;
                            }
                        }
                    }

                    DragEndPosition = TouchPosition;

                    IsTouchEnd = true;

                    brake = false;

                    Inertia = (speed > 0.0f);
                }

                if (Input.touches[0].phase == TouchPhase.Stationary)
                {
                    if (!isStay)
                    {
                        isStay = true;

                        holdStartTime = Time.time;
                    }

                    IsHolding = (Time.time - holdStartTime) > holdingDetectTime;
                }
                else
                {
                    isStay = false;
                }

                postTouchPosition = TouchPosition;                
            }
            else
            {
                lastDelta = Vector2.zero;

				IsDragging = false;
            }

            if (Inertia)
            {
                if (brake)
                {
                    speed -= brakeResistance * speed * resistance * Time.deltaTime;
                }
                else
                {
                    speed -= speed * resistance * Time.deltaTime;
                }

                if (speed < epsilon)
                {
                    Delta = Vector2.zero;

                    Inertia = false;

                    IsInertiaEnd = true;

                    speed = 0f;
                }

                speed = Mathf.Min(maxSpeed, speed);

                Delta = speed * lastVec * Time.deltaTime;
			}
			
            #region UnityEditor

#if UNITY_EDITOR
            TouchPosition = Input.mousePosition;

            if (Input.GetMouseButtonDown(0))
            {
				isStay = true;

                holdStartTime = Time.time;
                
                stayStartPosition = Input.mousePosition;

                DragStartPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                IsTouching = true;

                IsHolding = isStay & ((Time.time - holdStartTime) > holdingDetectTime);
                
                IsDragging = Vector2.Distance(Input.mousePosition, stayStartPosition) > dragDetectDistance;

                if (!postIsDragging && IsDragging)
                {
                    float h = Mathf.Abs(Input.mousePosition.x - stayStartPosition.x);

                    float v = Mathf.Abs(Input.mousePosition.y - stayStartPosition.y);

                    DragDirec = (h > v) ? DragDirection.Horizontal : DragDirection.Vertical;
                }
            }
            else if (Vector2.Distance(Input.mousePosition, stayStartPosition) > stayMagnitude)
            {
                IsHolding = false;
                
                isStay = false;                
            }
            else
            {
                IsHolding = false;

                isStay = false;
            }

            IsTouchBegin = Input.GetMouseButtonDown(0);

            IsTouchEnd = Input.GetMouseButtonUp(0);

            Delta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - postMousePosition;
#endif
            if (OnSwipe != null)
            {
                Vector2 lastVec = lastDelta / Time.deltaTime;

                if (lastVec.x > swipeSpeed)
                {
                    OnSwipe(new FASSwipeInfo(touchStartPosition, TouchPosition, FASSwipeInfo.Direction.Right));
                }
                else if (lastVec.x < -swipeSpeed)
                {
                    OnSwipe(new FASSwipeInfo(touchStartPosition, TouchPosition, FASSwipeInfo.Direction.Left));
                }
                else if (lastVec.y < -swipeSpeed)
                {
                    OnSwipe(new FASSwipeInfo(touchStartPosition, TouchPosition, FASSwipeInfo.Direction.Up));
                }
                else if (lastVec.y > swipeSpeed)
                {
                    OnSwipe(new FASSwipeInfo(touchStartPosition, TouchPosition, FASSwipeInfo.Direction.Down));
                }
            }

            #endregion

            postMousePosition = Input.mousePosition;

            postIsDragging = IsDragging;

            Delta = new Vector2(Mathf.Clamp(Delta.x, -Screen.width * 0.2f, Screen.width * 0.2f), Mathf.Clamp(Delta.y, -Screen.height * 0.2f, Screen.height * 0.2f));
        }

        private static float brakeResistance = 0.0f;

		public static void InertiaBrake(float resistance){

			Instance.brake = true;

            brakeResistance = Mathf.Abs(resistance);
		}

        private void DragBlockDown()
        {
            if(!IsPinching)
                dragBlock = false;
        }

		public static void Use(){

			Delta = Vector2.zero;
		}

		public static void Stop()
        {
			if (Instance == null) return;

			IsDragging = Inertia = false;
            IsHolding = false;
            IsTouching = false;
            IsInertiaEnd = false;
            
            Delta = Vector2.zero;
            
			Instance.isStay = false;
			Instance.lastDelta = Vector2.zero;
			Instance.speed = 0f;
			Instance.brake = false;

            IsPinching = false;

            //Input.ResetInputAxes();
        }

        public static void Pause()
        {
			if (Instance == null) return;

            Stop();

			Instance.pause = true;		
        }

        public static void Resume()
        {
			if (Instance == null) return;

			Instance.pause = false;
        }

		void OnApplicationPause(bool paused)
        {
			Stop();

			Resume();
		}

	}
}
