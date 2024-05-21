using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Система оружия
	/// </summary>
	[Component (PropertyGuid = "a1a26a6b3e63b742fd5d7a614f237c071f8bdc95")]
	public class System_Weapon : Component {
		[ShowInEditor, ParameterSwitch (Title = "Тип оружия", Items = "Строитель,Дальний бой")]
		private int weaponType = 0;

		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Объект макета")]
		private Node weaponBuilder_ObjectMaket = null;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Объект строительства")]
		private Node weaponBuilder_Object = null;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Отступ объекта")]
		private float weaponBuilder_Offset = 5;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Скорость вращения")]
		private float weaponBuilder_SpeedRotate = 3;
		
		void Init () {
			System_Inputs.mouseFire += WeaponFire;
			System_Inputs.mouseAim += WeaponAim;
			System_Inputs.OnChangeMouseWheel_Event += WeaponBuilder_Rotation;
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) weaponBuilder_ObjectMaket.Enabled = false;
		}

		void Update () {
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null && weaponBuilder_ObjectMaket.Enabled) {
				WeaponBuilder_SetTransform ();
			}
		}
		
		void Shutdown () {
			System_Inputs.mouseFire -= WeaponFire;
			System_Inputs.mouseAim -= WeaponAim;
			System_Inputs.OnChangeMouseWheel_Event -= WeaponBuilder_Rotation;
		}

		/// <summary>
		/// Событие атаки
		/// </summary>
		void WeaponFire () {
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) {
				if (!System_Inputs.weaponActive) {
					//Начало строительства
					System_Inputs.weaponActive = true;

					weaponBuilder_ObjectMaket.Parent = null;
					weaponBuilder_ObjectMaket.Enabled = true;
				} else {
					//Строительство
					Node _newObject = weaponBuilder_Object.Clone ();
					_newObject.Parent = null;
					_newObject.WorldPosition = weaponBuilder_ObjectMaket.WorldPosition;
					_newObject.SetWorldRotation (weaponBuilder_ObjectMaket.GetWorldRotation ());
					_newObject.Enabled = true;

					WeaponAim ();
				}
			}
		}
		
		/// <summary>
		/// Событие прицеливание
		/// </summary>
		void WeaponAim () {
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) {
				System_Inputs.weaponActive = false;
				
				weaponBuilder_ObjectMaket.Parent = node;
				weaponBuilder_ObjectMaket.Enabled = false;
			}
		}

		/// <summary>
		/// Установка объекта для строительства
		/// </summary>
		void WeaponBuilder_SetTransform () {
			//Позиция
			mat4 _nodeTransform = node.WorldTransform;
			vec3 _newPosition = node.WorldPosition + (_nodeTransform.GetColumn3 (1) * weaponBuilder_Offset);
			
			weaponBuilder_ObjectMaket.WorldPosition = _newPosition;
			//Поворот
			/*vec3 _targetRot = node.GetWorldRotation ().Euler;

			weaponBuilder_Object.SetWorldRotation (new quat (_targetRot.x, _targetRot.y, _targetRot.z));*/
		}
		
		/// <summary>
		/// Вращение объекта строительства
		/// </summary>
		/// <param name="value">
		/// Следующее значение
		/// </param>
		void WeaponBuilder_Rotation (int value) {
			if (!System_Inputs.weaponActive || weaponType != 0 || weaponBuilder_ObjectMaket == null) return;
			
			weaponBuilder_ObjectMaket.Rotate (value * weaponBuilder_SpeedRotate, 0, 0);
		}
	}
}