using System;
using RiboShock.Systems;
using Unigine;

/// <summary>
/// Основной раздел
/// </summary>
namespace RiboShock {
	/// <summary>
	/// Игровой менеджер
	/// </summary>
	[Component (PropertyGuid = "57eba3dc6afedf451ddcf06c50374d05d7e748bf")]
	public class Game_Manager : Component {
		[Parameter (Title = "Настройки ввода", Tooltip = "Кастомные настройки ввода")]
		public CustomKeyAssignment customKeyAssignment;
		/// <summary>
		/// Настройки ввода
		/// </summary>
		public static CustomKeyAssignment customKeyAssignment_Static { private set; get; }
		
		void Init () {
			Unigine.Console.Onscreen = true;
			//Настройка импута
			customKeyAssignment_Static = customKeyAssignment;
			
			System_Inputs.SetMouseGrab (true);
			System_Inputs.SetCustomInput (customKeyAssignment);
		}
		
		void Shutdown () {
			Unigine.Console.Onscreen = false;
		}
	}
	
	/// <summary>
	/// Кастомнаое назначение клавиш
	/// </summary>
	[Serializable]
	public class CustomKeyAssignment {
		//Клавиатура
		[Parameter (Group = "Клавиатура", Title = "Отмена")]
		public Input.KEY cancelKey = Input.KEY.ESC;

		[Parameter (Group = "Клавиатура", Title = "Движение вперед")]
		public Input.KEY moveForwardKey = Input.KEY.W;
		[Parameter (Group = "Клавиатура", Title = "Движение назад")]
		public Input.KEY moveBackwardKey = Input.KEY.S;
		[Parameter (Group = "Клавиатура", Title = "Движение влево")]
		public Input.KEY moveLeftKey = Input.KEY.A;
		[Parameter (Group = "Клавиатура", Title = "Движение вправо")]
		public Input.KEY moveRightKey = Input.KEY.D;
		
		[Parameter (Group = "Клавиатура", Title = "Табуляция")]
		public Input.KEY tabulationKey = Input.KEY.TAB;
		[Parameter (Group = "Клавиатура", Title = "Прыжок")]
		public Input.KEY jumpKey = Input.KEY.SPACE;
		[Parameter (Group = "Клавиатура", Title = "Ускорение")]
		public Input.KEY runKey = Input.KEY.ANY_SHIFT;
		[Parameter (Group = "Клавиатура", Title = "Замедление")]
		public Input.KEY brakingKey = Input.KEY.ANY_CTRL;

		[Parameter (Group = "Клавиатура", Title = "Взаимодествие")]
		public Input.KEY interactionKey = Input.KEY.F;
		[Parameter (Group = "Клавиатура", Title = "Инвентарь")]
		public Input.KEY inventoryKey = Input.KEY.I;
		[Parameter (Group = "Клавиатура", Title = "Ячейки")]
		public Input.KEY [] inventorySlots = new [] {
			Input.KEY.ANY_DIGIT_1,
			Input.KEY.ANY_DIGIT_2
		};
		
		//Мышь
		[Parameter (Group = "Мышь", Title = "Атака/Огонь")]
		public Input.MOUSE_BUTTON mouseFireKey = Input.MOUSE_BUTTON.LEFT;
	}
}