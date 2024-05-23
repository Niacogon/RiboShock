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
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Дистанция уничтожения")]
		private float rayDistance = 100;
		/// <summary>
		/// Текущий поворот по оси X
		/// </summary>
		private float weaponBuilder_CurAxisX = 90;
		private WorldIntersection ray;
		private Unigine.Object rayObj;
		
		void Init () {
			System_Inputs.mouseFire += WeaponFire;

			if (weaponType != 0) System_Inputs.mouseAim += WeaponAim;
			else WeaponFire ();

			System_Inputs.onDestroy += WeaponBuilder_Destroy;
			System_Inputs.OnChangeMouseWheel_Event += WeaponBuilder_Rotation;
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) weaponBuilder_ObjectMaket.Enabled = false;
		}

		void Update () {
			WeaponBuilder_SetTransform ();
		}
		
		void Shutdown () {
			System_Inputs.mouseFire -= WeaponFire;
			
			if (weaponType != 0) System_Inputs.mouseAim -= WeaponAim;
			
			System_Inputs.onDestroy -= WeaponBuilder_Destroy;
			System_Inputs.OnChangeMouseWheel_Event -= WeaponBuilder_Rotation;
		}

		/// <summary>
		/// Событие атаки
		/// </summary>
		void WeaponFire () {
			//Строитель
			if (weaponType == 0 && System_Inputs.weaponActive) {
				if (weaponBuilder_ObjectMaket != null) {
					//Строительство
					Node _newObject = weaponBuilder_Object.Clone ();
					_newObject.Parent = null;
					_newObject.WorldPosition = weaponBuilder_ObjectMaket.WorldPosition;
					_newObject.SetWorldRotation (weaponBuilder_ObjectMaket.GetWorldRotation ());
					_newObject.Enabled = true;
				}
			}
		}
		
		/// <summary>
		/// Событие прицеливание
		/// </summary>
		void WeaponAim () {
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) {
				/*System_Inputs.weaponActive = false;
				
				weaponBuilder_ObjectMaket.Parent = node;
				weaponBuilder_ObjectMaket.Enabled = false;*/
			}
		}
		
		/// <summary>
		/// Активация строителя
		/// </summary>
		/// <param name="active">
		/// Активен?
		/// </param>
		public void WeaponBuilder_Active (bool active) {
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) {
				System_Inputs.weaponActive = active;

				weaponBuilder_ObjectMaket.Parent = active ? null : node;
				weaponBuilder_ObjectMaket.Enabled = active;
			}
		}

		/// <summary>
		/// Установка объекта для строительства
		/// </summary>
		void WeaponBuilder_SetTransform () {
			if (!System_Inputs.weaponActive || weaponType != 0 || weaponBuilder_ObjectMaket == null) return;
			
			weaponBuilder_ObjectMaket.Enabled = true;
			//Позиция
			vec3 _newPosition = node.WorldPosition + (node.WorldTransform.GetColumn3 (1) * weaponBuilder_Offset);
			weaponBuilder_ObjectMaket.WorldPosition = _newPosition;
			//Поворот
			vec3 _targetDir = weaponBuilder_ObjectMaket.WorldPosition - node.WorldPosition;
			weaponBuilder_ObjectMaket.SetWorldDirection (_targetDir, vec3.UP, MathLib.AXIS.Y);

			weaponBuilder_ObjectMaket.Rotate (weaponBuilder_CurAxisX, 0, 0);
		}
		
		/// <summary>
		/// Вращение объекта строительства
		/// </summary>
		/// <param name="value">
		/// Следующее значение
		/// </param>
		void WeaponBuilder_Rotation (int value) {
			if (!System_Inputs.weaponActive || weaponType != 0 || weaponBuilder_ObjectMaket == null) return;

			weaponBuilder_CurAxisX += value * weaponBuilder_SpeedRotate;
			
			if (weaponBuilder_CurAxisX >= 360) weaponBuilder_CurAxisX = 0;
			if (weaponBuilder_CurAxisX < 0) weaponBuilder_CurAxisX += 360;
		}

		/// <summary>
		/// Уничножение объекта
		/// </summary>
		void WeaponBuilder_Destroy () {
			if (weaponType != 0) return;
			
			vec3 _start = Engine.MainPlayer.WorldPosition;
			vec3 _end = Engine.MainPlayer.GetWorldDirection () * rayDistance;
			rayObj = World.GetIntersection (_start, _end, 1, ray);

			if (rayObj) {
				if (rayObj.RootNode) rayObj.RootNode.DeleteLater ();
				else rayObj.DeleteLater ();
			}
		}
	}
}