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
		[ShowInEditor, ParameterSwitch (Title = "Тип оружия", Items = "Строитель,Дальний бой,Граната")]
		private int weaponType = 0;

		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Объект макета")]
		private Node weaponBuilder_ObjectMaket = null;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Объект строительства")]
		private Node weaponBuilder_Object = null;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Отступ объекта")]
		private float weaponBuilder_Offset = 5;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Скорость вращения")]
		private float weaponBuilder_SpeedRotate = 15;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 0), Parameter (Group = "Строитель", Title = "Дистанция уничтожения")]
		private float rayDistance = 100;
		/// <summary>
		/// Текущий поворот по оси X
		/// </summary>
		private float weaponBuilder_CurAxisX = 90;
		private WorldIntersection ray;
		private Unigine.Object rayObj;

		[ShowInEditor, ParameterCondition (nameof (weaponType), 2), Parameter (Group = "Граната", Title = "Активна?")]
		private bool weaponGrenade_isActive = true;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 2), Parameter (Group = "Граната", Title = "Сила броска")]
		private float weaponGrenade_Impuls = 1500f;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 2), Parameter (Group = "Граната", Title = "Время уничтожения")]
		private float weaponGrenade_Time = .03f;
		/// <summary>
		/// Время уничтожения
		/// </summary>
		private float weaponGrenade_TimeDestruction = 0f;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 2), Parameter (Group = "Граната", Title = "Физический ипульс")]
		private PhysicalForce weaponGrenade_PhysicalForce = null;
		[ShowInEditor, ParameterCondition (nameof (weaponType), 2), ParameterAsset (Group = "Граната", Title = "Нода гранаты")]
		private AssetLink weaponGrenade_SpawnNode = null;

		/// <summary>
		/// Физическое тело
		/// </summary>
		private Body body;

		void Init () {
			System_Inputs.mouseFire += WeaponFire;

			if (weaponType != 0) System_Inputs.mouseAim += WeaponAim;
			else WeaponFire ();

			System_Inputs.onDestroy += WeaponBuilder_Destroy;
			System_Inputs.OnChangeMouseWheel_Event += WeaponBuilder_Rotation;
			
			body = node.ObjectBody;
			//Строитель
			if (weaponType == 0 && weaponBuilder_ObjectMaket != null) weaponBuilder_ObjectMaket.Enabled = false;
			//Граната
			if (weaponType == 2) body.EventContactEnter.Connect (WeaponGrenade_Detonation);
		}

		void Update () {
			//Строитель
			if (weaponType == 0) WeaponBuilder_SetTransform ();
			//Граната
			if (weaponType == 2 && weaponGrenade_PhysicalForce.Enabled) {
				weaponGrenade_TimeDestruction += Game.IFps;
				//Уничтожение гранаты
				if (weaponGrenade_TimeDestruction > weaponGrenade_Time)
					node.DeleteLater ();
			}
		}

		void Shutdown () {
			System_Inputs.mouseFire -= WeaponFire;

			if (weaponType != 0) System_Inputs.mouseAim -= WeaponAim;

			System_Inputs.onDestroy -= WeaponBuilder_Destroy;
			System_Inputs.OnChangeMouseWheel_Event -= WeaponBuilder_Rotation;
			//Граната
			if (weaponType == 2) body.EventContactEnter.Disconnect (WeaponGrenade_Detonation);
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
			if (weaponType == 0) {
				vec3 _start = Engine.MainPlayer.WorldPosition;
				vec3 _end = Engine.MainPlayer.GetWorldDirection () * rayDistance;
				rayObj = World.GetIntersection (_start, _end, 1, ray);

				if (rayObj) {
					if (rayObj.RootNode) rayObj.RootNode.DeleteLater ();
					else rayObj.DeleteLater ();
				}
			}
		}

		/// <summary>
		/// Появление гранаты
		/// </summary>
		/// <param name="spawnPosition">
		/// Точка появления
		/// </param>
		public void WeaponGrenade_Spawn (vec3 spawnPosition) {
			Node _newGranade = World.LoadNode (weaponGrenade_SpawnNode.AbsolutePath);
			_newGranade.WorldPosition = spawnPosition;
			_newGranade.ObjectBodyRigid.AddForce (Engine.MainPlayer.GetWorldDirection () * weaponGrenade_Impuls);
		}

		/// <summary>
		/// Детонация гранаты
		/// </summary>
		void WeaponGrenade_Detonation () {
			//Дитонация
			weaponGrenade_PhysicalForce.Enabled = true;
		}
	}
}