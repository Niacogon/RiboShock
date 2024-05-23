using System;
using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Система ввода
	/// </summary>
	[Component (PropertyGuid = "36603f58e39275b3c1d71824719108bab98a637c")]
	public class System_Inputs : Component {
		/// <summary>
		/// Кастомнаое назначение клавиш
		/// </summary>
		static CustomKeyAssignment customKeyAssignment = new CustomKeyAssignment ();

		/// <summary>
		/// Установка кастомнаое назначение клавиш
		/// </summary>
		public static void SetCustomInput (CustomKeyAssignment value) => customKeyAssignment = value;
		static float changeTypeMove_Timer = .5f, changeTypeMove_TimerStart = .5f;

		/// <summary>
		/// Обновление системы ввода
		/// </summary>
		void Update () {
			if (Unigine.Console.Active) return;
			//клавиатура
			SetCancel ();
			SetMoveAxis ();
			SetJumping ();

			SetTabulation ();
			SetInteraction ();
			SetDestroy ();
			SetInventoryActive ();
			SetSlot ();
			//мышь
			Input.MouseGrab = mouseGrab;
			Input.MouseCursorHide = mouseGrab;
			
			SetMouseAxis ();
			SetMouseFire ();
			SetMouseAim ();
			SetMouseWheel ();
		}

		#region Keyboard
		/// <summary>
		/// Отмнена
		/// </summary>
		public static Action onCancel;
		/// <summary>
		/// Собюытие отмнены
		/// </summary>
		static void SetCancel () {
			if (Input.IsKeyDown (customKeyAssignment.cancelKey)) {
				onCancel?.Invoke ();
				SetMouseGrab (mouseGrab ? false : true);
			}
		}

		/// <summary>
		/// Ось движения
		/// </summary>
		public static vec3 moveAxis { private set; get; } = new vec3 (0, 0, 2);
		public delegate void ChangeMoveAxis (vec3 value);
		/// <summary>
		/// Событие изменения движения оси
		/// </summary>
		public static event ChangeMoveAxis OnChangeMoveAxis_Event;
		/// <summary>
		/// Установка оси движения
		/// </summary>
		static void SetMoveAxis () {
			float _vy = Input.IsKeyPressed (customKeyAssignment.moveForwardKey) ? 1 :
				Input.IsKeyPressed (customKeyAssignment.moveBackwardKey) ? -1 : 0;
			float _vx = Input.IsKeyPressed (customKeyAssignment.moveRightKey) ? 1 :
				Input.IsKeyPressed (customKeyAssignment.moveLeftKey) ? -1 : 0;

			float _vz = Input.IsKeyDown (customKeyAssignment.runKey) && moveAxis.z <= 1
				? moveAxis.z + 1
				: Input.IsKeyDown (customKeyAssignment.brakingKey)
					? moveAxis.z - 1
					: Input.IsKeyPressed (customKeyAssignment.runKey) && changeTypeMove_Timer <= 0 && moveAxis.z == 2 &&
					  _vx == 0 && _vy > 0
						? moveAxis.z + 1
						: moveAxis.z == 3 && (!Input.IsKeyPressed (customKeyAssignment.runKey) || _vx != 0 || _vy <= 0)
							? moveAxis.z - 1
							: moveAxis.z;

			_vz = MathLib.Clamp (_vz, 0f, 3f);
			if (!Unigine.Console.Active) moveAxis = new vec3 (_vx, _vy, _vz);
			else moveAxis = vec3.ZERO;

			OnChangeMoveAxis_Event?.Invoke (moveAxis);
			if (_vz != 2) changeTypeMove_Timer = changeTypeMove_TimerStart;
			if (_vz == 2 && changeTypeMove_Timer > 0) changeTypeMove_Timer -= Game.IFps;
		}

		/// <summary>
		/// Прыжок
		/// </summary>
		public static bool jump { private set; get; }
		public delegate void ChangeJump (bool value);
		/// <summary>
		/// Событие изменения состояния прыжка
		/// </summary>
		public static event ChangeJump OnChangeJump_Event;
		/// <summary>
		/// Установка прыжка
		/// </summary>
		static void SetJumping () {
			jump = Input.IsKeyDown (customKeyAssignment.jumpKey) ? true : false;
			OnChangeJump_Event?.Invoke (jump);
		}

		/// <summary>
		/// Табуляция
		/// </summary>
		public static bool onTabulation { private set; get; }
		public delegate void ChangeTabulation (bool value);
		/// <summary>
		/// Событие изменения табуляции
		/// </summary>
		public static event ChangeTabulation OnChangeTabulation_Event;
		/// <summary>
		/// Установка табуляции
		/// </summary>
		static void SetTabulation () {
			onTabulation = Input.IsKeyDown (customKeyAssignment.tabulationKey) ? true : false;
			OnChangeTabulation_Event?.Invoke (onTabulation);
		}

		/// <summary>
		/// Взаимодействие
		/// </summary>
		public static Action onInteraction;
		/// <summary>
		/// Собюытие взаимодействия
		/// </summary>
		static void SetInteraction () {
			if (Input.IsKeyDown (customKeyAssignment.interactionKey))
				onInteraction?.Invoke ();
		}
		
		/// <summary>
		/// Уничтожение
		/// </summary>
		public static Action onDestroy;
		/// <summary>
		/// Собюытие уничтожения
		/// </summary>
		static void SetDestroy () {
			if (mouseGrab && Input.IsKeyDown (customKeyAssignment.destroyKey))
				onDestroy?.Invoke ();
		}
		
		/// <summary>
		/// Инвентарь
		/// </summary>
		public static bool inventoryActive;// { private set; get; }
		public delegate void ChangeInventoryActive (bool value);
		/// <summary>
		/// Событие изменения табуляции
		/// </summary>
		public static event ChangeInventoryActive OnChangeInventoryActive_Event;
		/// <summary>
		/// Установка табуляции
		/// </summary>
		static void SetInventoryActive () {
			if (Input.IsKeyDown (customKeyAssignment.inventoryKey)) inventoryActive = inventoryActive ? false : true;
			if (inventoryActive == mouseGrab) SetMouseGrab (!inventoryActive);
			OnChangeInventoryActive_Event?.Invoke (inventoryActive);
		}
		
		/// <summary>
		/// Слоты
		/// </summary>
		public static Action <int> onSlot;
		/// <summary>
		/// Собюытие слотов
		/// </summary>
		static void SetSlot () {
			for (int _s = 0; _s < customKeyAssignment.inventorySlots.Length; _s++) {
				if (Input.IsKeyDown (customKeyAssignment.inventorySlots [_s]))
					onSlot?.Invoke (_s);
			}
		}
		#endregion

		#region Mouse
		/// <summary>
		/// Активность мыши
		/// </summary>
		public static bool mouseGrab { private set; get; }
		/// <summary>
		/// Позиция мыши при скрытии
		/// </summary>
		static ivec2 mouseHandlePos;
		/// <summary>
		/// Установка активности мыши
		/// </summary>
		/// <param name="mouseAxis">
		/// Активна?
		/// </param>
		public static void SetMouseGrab (bool enabled) {
			if (enabled) mouseHandlePos = Input.MousePosition;
			else Input.MousePosition = mouseHandlePos;
			mouseGrab = enabled;
		}

		/// <summary>
		/// Ось мыши
		/// </summary>
		public static vec2 mouseAxis { private set; get; } = new vec2 ();
		public delegate void ChangeMouseAxis (vec2 value);
		/// <summary>
		/// Событие изменения оси мыши
		/// </summary>
		public static event ChangeMouseAxis OnChangeMouseAxis_Event;
		/// <summary>
		/// Установка оси мыши
		/// </summary>
		static void SetMouseAxis () {
			if (!Unigine.Console.Active) mouseAxis = Input.MouseDeltaRaw;
			else mouseAxis = vec2.ZERO;

			OnChangeMouseAxis_Event?.Invoke (mouseAxis);
		}

		/// <summary>
		/// Событие атаки
		/// </summary>
		public static Action mouseFire;
		/// <summary>
		/// Установка события атаки
		/// </summary>
		static void SetMouseFire () {
			if (mouseGrab && Input.IsMouseButtonDown (customKeyAssignment.mouseFire)) 
				mouseFire?.Invoke ();
		}

		/// <summary>
		/// Событие прицеливания
		/// </summary>
		public static Action mouseAim;
		/// <summary>
		/// Установка события прицеливания
		/// </summary>
		static void SetMouseAim () {
			if (mouseGrab && Input.IsMouseButtonDown (customKeyAssignment.mouseAim)) 
				mouseAim?.Invoke ();
		}

		/// <summary>
		/// Активна ли оружние (для блокировки смены оружия через колесико мыши)
		/// </summary>
		public static bool weaponActive = false;

		/// <summary>
		/// Значение колёсика мыши
		/// </summary>
		public static int mouseWheel { private set; get; }
		public delegate void ChangeMouseWheel (int value);
		/// <summary>
		/// Событие изменения значения колёсика мыши
		/// </summary>
		public static event ChangeMouseWheel OnChangeMouseWheel_Event;
		/// <summary>
		/// Установка значения колёсика мыши
		/// </summary>
		static void SetMouseWheel () {
			mouseWheel = Input.MouseWheel;
			if (mouseWheel != 0) OnChangeMouseWheel_Event?.Invoke (mouseWheel);
		}
		#endregion
	}
}