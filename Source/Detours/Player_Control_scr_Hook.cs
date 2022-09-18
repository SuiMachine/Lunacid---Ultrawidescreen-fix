using HarmonyLib;
using System;
using UnityEngine;

namespace LunacidUltrawidescreenFix.Detours
{
	[HarmonyPatch]
	public static class Player_Control_scr_Hook
	{
		//Original had the values multiplied by 1/60 in 1/50 Hz update - so having Time.delta * 0.8333333333333333f should hopefully be correct
		private const float CONST_ROTATION_MULTIPLIER = 0.8333333333333333f;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(Player_Control_scr), "Update")]
		public static void UpdateHook(Player_Control_scr __instance, ref Vector3 ___OLD_ROT)
		{
			__instance.ROT.x = __instance.ROT.x + __instance.New_rotation.y * Time.deltaTime * CONST_ROTATION_MULTIPLIER;
			__instance.ROT.y = __instance.ROT.y + __instance.New_rotation.x * Time.deltaTime * CONST_ROTATION_MULTIPLIER;

			if (__instance.ROT.y > 180f)
			{
				__instance.ROT.y = __instance.ROT.y - 360f;
				___OLD_ROT.y = ___OLD_ROT.y - 360f;
			}
			else if (__instance.ROT.y < -180f)
			{
				__instance.ROT.y = __instance.ROT.y + 360f;
				___OLD_ROT.y = ___OLD_ROT.y + 360f;
			}

			__instance.ROT.x = Mathf.Clamp(__instance.ROT.x, -80f, 80f);
			__instance.transform.rotation = Quaternion.Euler(0f, __instance.ROT.y, 0f);
			__instance.HEAD.transform.localRotation = Quaternion.Euler(__instance.ROT.x, 0f, 0f);
			___OLD_ROT = __instance.ROT;
			__instance.magni = Quaternion.Angle(__instance.HANDS.transform.localRotation, Quaternion.Euler(__instance.ROT)) * 0.25f;
			__instance.HANDS.transform.localRotation = Quaternion.Slerp(__instance.HANDS.transform.localRotation, Quaternion.Euler(__instance.ROT), Time.deltaTime * CONST_ROTATION_MULTIPLIER * __instance.magni);
		}


		[HarmonyPrefix]
		[HarmonyPatch(typeof(Player_Control_scr), "FixedUpdate")]
		public static bool FixedUpdateHook(Player_Control_scr __instance, ref Vector2 ___Normalize_Movement, ref bool ___sprinting, ref bool ___grounded)
		{
			if (!__instance.Freeze)
			{
				if (__instance.CON.CURRENT_PL_DATA.PLAYER_H > __instance.CON.CURRENT_PL_DATA.PLAYER_B)
				{
					if (__instance.CON.CURRENT_PL_DATA.PLAYER_CLASS.ToUpper() == "VAMPIRE")
						__instance.CON.CURRENT_PL_DATA.PLAYER_B += 0.0025f * Time.deltaTime * __instance.CON.PLAYER_MAX_HP;
					else
						__instance.CON.CURRENT_PL_DATA.PLAYER_B += 0.02f * Time.deltaTime * __instance.CON.PLAYER_MAX_HP;

					if (__instance.CON.CURRENT_PL_DATA.PLAYER_B > __instance.CON.CURRENT_PL_DATA.PLAYER_H)
						__instance.CON.CURRENT_PL_DATA.PLAYER_B = __instance.CON.CURRENT_PL_DATA.PLAYER_H;
					else if (__instance.CON.CURRENT_PL_DATA.PLAYER_B <= 0f)
						__instance.Die();
				}

				if (__instance.CON.CURRENT_PL_DATA.PLAYER_M < __instance.CON.PLAYER_MAX_MP && __instance.CON.CURRENT_PL_DATA.PLAYER_RES >= 100)
					__instance.CON.CURRENT_PL_DATA.PLAYER_M += 0.3f;
			}

			___Normalize_Movement = new Vector2(__instance.vertical, __instance.horizontal);
			if (___Normalize_Movement.magnitude > 1)
			{
				___Normalize_Movement = ___Normalize_Movement.normalized;
				__instance.vertical = ___Normalize_Movement.x;
				__instance.horizontal = ___Normalize_Movement.y;
			}

			if (!__instance.Swimming)
			{
				Vector3 vector = new Vector3(__instance.horizontal, 0f, __instance.vertical);
				vector = __instance.transform.TransformDirection(vector);

				if (__instance.CON.CURRENT_SYS_DATA.SETTING_RUN == 1)
				{
					if (___sprinting)
						vector *= __instance.speed;
					else
						vector *= 3f;
				}
				else if (!___sprinting)
					vector *= __instance.speed;
				else
					vector *= 3f;

				Vector3 velocity = __instance.BODY.velocity;
				Vector3 vector2 = vector - velocity;

				if (!___grounded)
					vector2 *= __instance.air_control;

				vector2.x = Mathf.Clamp(vector2.x, -__instance.max_acel, __instance.max_acel);
				vector2.z = Mathf.Clamp(vector2.z, -__instance.max_acel, __instance.max_acel);
				vector2.y = 0f;
				__instance.BODY.AddForce(vector2, ForceMode.VelocityChange);
			}
			else if (__instance.Swimming)
			{
				Vector3 vector3 = new Vector3(__instance.horizontal, __instance.ROT.x * __instance.vertical * -0.01f, __instance.vertical);
				vector3 = __instance.transform.TransformDirection(vector3);

				if (__instance.CON.CURRENT_SYS_DATA.SETTING_RUN == 1)
				{
					if (___sprinting)
						vector3 *= __instance.speed;
					else
						vector3 *= 3f;
				}
				else if (!___sprinting)
					vector3 *= __instance.speed;
				else
					vector3 *= 3f;

				Vector3 velocity2 = __instance.BODY.velocity;
				Vector3 vector4 = vector3 - velocity2;
				vector4.x = Mathf.Clamp(vector4.x, -__instance.max_acel, __instance.max_acel);
				vector4.z = Mathf.Clamp(vector4.z, -__instance.max_acel, __instance.max_acel);
				__instance.BODY.AddForce(vector4, ForceMode.VelocityChange);
			}

			__instance.BODY.AddForce(new Vector3(0f, -__instance.gravity * __instance.BODY.mass, 0f));
			return false;
		}
	}
}
