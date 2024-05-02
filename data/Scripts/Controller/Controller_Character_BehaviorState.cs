using System;
using Unigine;

/// <summary>
/// Раздел котроллеров
/// </summary>
namespace RiboShock.Controller {
	/// <summary>
	/// Состояние поведения персонажа
	/// </summary>
	[Serializable, Component (PropertyGuid = "ce430d91efed54816db776953d8e54c16c7a5999")]
	public class Controller_Character_BehaviorState {
		/// <summary>
		/// Поведение
		/// </summary>
		public enum Behavior {
			Forward = 0,
			Backward,
			Left,
			Right,

			Crouch,
			Walk,
			Run,
			Sprint,

			Jump,
			Fire,

			NumStates
		};
		/// <summary>
		/// Состояние статуса
		/// </summary>
		public enum StateStatus {
			Diasbled = 0,
			Enabled,
			Begin,
			End
		};

		/// <summary>
		/// Номера статусов
		/// </summary>
		public int [] states = new int [(int) Behavior.NumStates];
		/// <summary>
		/// Время статусов
		/// </summary>
		public float [] times = new float [(int) Behavior.NumStates];
	}
}