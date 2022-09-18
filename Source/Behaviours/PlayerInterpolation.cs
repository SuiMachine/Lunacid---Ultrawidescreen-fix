using System.Collections;
using UnityEngine;

namespace LunacidUltrawidescreenFix.Behaviours
{
	[DefaultExecutionOrder(500)]
	public class PlayerInterpolation : MonoBehaviour
	{
		public PositionHistory[] History = new PositionHistory[2];

		private Coroutine PostFrameRenderAction;
		private Player_Control_scr PlayerCtrlRef;

		void OnEnable()
		{
			PlayerCtrlRef = GetComponent<Player_Control_scr>();
			ClearHistory();
		}

		void Update()
		{
			if (PlayerCtrlRef == null)
				return;

			if (!History[0].WasActive || !History[1].WasActive)
				return;

			float newerTime = History[0].StoredTime;
			float olderTime = History[1].StoredTime;
			if (newerTime != olderTime)
			{
				var interpolationFactor = (Time.time - newerTime) / (newerTime - olderTime);
				PlayerCtrlRef.BODY.transform.position = Vector3.LerpUnclamped(History[1].Position, History[0].Position, interpolationFactor);
				PlayerCtrlRef.BODY.transform.localScale = Vector3.LerpUnclamped(History[1].LocalScale, History[0].LocalScale, interpolationFactor);
			}

			if (PostFrameRenderAction != null)
				StopCoroutine(PostFrameRenderAction);

			PostFrameRenderAction = StartCoroutine(RestoreOriginalPostFrame());
		}

		IEnumerator RestoreOriginalPostFrame()
		{
			yield return new WaitForEndOfFrame();
			PlayerCtrlRef.BODY.transform.position = History[0].Position;
			PlayerCtrlRef.BODY.transform.localScale = History[0].LocalScale;
			PostFrameRenderAction = null;
		}


		void FixedUpdate()
		{
			History[1] = History[0];
			History[0] = new PositionHistory(PlayerCtrlRef.BODY.transform);
		}


		private void ClearHistory()
		{
			History[0] = new PositionHistory();
			History[1] = new PositionHistory();
		}

	}
}
