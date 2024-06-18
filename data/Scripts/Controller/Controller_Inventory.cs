using System.Collections.Generic;
using Unigine;
using RiboShock.Systems;

/// <summary>
/// Раздел котроллеров
/// </summary>
namespace RiboShock.Controller {
	/// <summary>
	/// Контроллер инвентаря
	/// </summary>
	[Component (PropertyGuid = "92699568127d9246cff330608d843f09990b8949")]
	public class Controller_Inventory : Component {
		[ShowInEditor, Parameter (Title = "Хранилище предметов")]
		private Node parentItems;
		[ShowInEditor, Parameter (Title = "Точка спавна")]
		private Node spawnPoint;
		
		/// <summary>
		/// Список предметов
		/// </summary>
		private List <System_Item> items = new List <System_Item> ();

		/// <summary>
		/// Выбранное оружие
		/// </summary>
		public int weaponSelect { private set; get; } = -1;
		/// <summary>
		/// Первое оружие
		/// </summary>
		private System_Item weaponFirst;
		/// <summary>
		/// Второе оружие
		/// </summary>
		private System_Item weaponSecond;
		/// <summary>
		/// Гранаты
		/// </summary>
		private List <System_Item> weaponGrenade = new List <System_Item> ();

		void Init () {
			System_Inputs.onSlot += SelectWeapon_Cell;
			System_Inputs.OnChangeMouseWheel_Event += SelectWeapon_Wheel;
			System_Inputs.OnGrenadeActive_Event += SelectGrenade;
		}

		void Shutdown () {
			System_Inputs.onSlot -= SelectWeapon_Cell;
			System_Inputs.OnChangeMouseWheel_Event -= SelectWeapon_Wheel;
			System_Inputs.OnGrenadeActive_Event -= SelectGrenade;
		}

		/// <summary>
		/// Добавление предмета
		/// </summary>
		/// <param name="newItem">
		/// Новый предмет
		/// </param>
		public void AddItem (System_Item newItem) {
			//Диактивация подбора
			newItem.body.Enabled = false;
			newItem.objectRotation.Enabled = false;
			newItem.objectWeapon.Enabled = false;
			//Установка объекта к точке
			newItem.node.Enabled = false;
			newItem.node.Parent = parentItems;
			newItem.node.Position = vec3.ZERO;
			newItem.node.SetRotation (quat.ZERO, true);
			newItem.node.Rotate (-90, 0, 0);
			//Добавление придмета в список
			items.Add (newItem);
			SortItemList ();
			//Определние типа оружия
			if (newItem.GetTypeItem () == 0) weaponFirst = newItem;
			if (newItem.GetTypeItem () == 1) weaponSecond = newItem;
			if (newItem.GetTypeItem () == 2) weaponGrenade.Add (newItem);
			//Выбор подоброного оружия
			if (weaponSelect == -1) {
				System_Inputs.onSlot?.Invoke (newItem.GetTypeItem ());
			}
		}
		
		/// <summary>
		/// Удаление предмета
		/// </summary>
		/// <param name="curItem">
		/// Текущий предмет
		/// </param>
		public void RemoveItem (System_Item curItem) {
			//Определние типа оружия
			if (curItem.GetTypeItem () == 0) weaponFirst = null;
			if (curItem.GetTypeItem () == 1) weaponSecond = null;
			if (curItem.GetTypeItem () == 2) weaponGrenade.Remove (curItem);
			//Позиция появления
			vec3 _spawnPosition = node.WorldPosition;
			_spawnPosition.z= .3f;
			//Появление предмета
			curItem.node.Parent = null;
			curItem.node.Position = _spawnPosition;
			curItem.node.SetRotation (quat.ZERO, true);
			//Активаци подбора предмета
			curItem.body.Enabled = true;
			curItem.objectRotation.Enabled = true;
			curItem.objectWeapon.Enabled = false;
			//Удаление из списка
			items.Remove (curItem);
		}

		/// <summary>
		/// Сортировка предметов
		/// </summary>
		void SortItemList () {
			items.Sort (delegate (System_Item x, System_Item y) {
				if (x == null && y == null) return 0;
				else if (x == null) return -1;
				else if (y == null) return 1;
				else return x.GetItemID ().CompareTo (y.GetItemID ());
			});
		}

		/// <summary>
		/// Выбор оружия через клавиатуру
		/// </summary>
		/// <param name="value">
		/// Текущая ячейка
		/// </param>
		void SelectWeapon_Cell (int value) {
			weaponSelect = value;
			ActiveWeapon ();
		}
		
		/// <summary>
		/// Выбор оружия через колесико мыши
		/// </summary>
		/// <param name="value">
		/// Следующее значение
		/// </param>
		void SelectWeapon_Wheel (int value) {
			if (System_Inputs.weaponActive) return;
			
			weaponSelect -= value;
			weaponSelect = MathLib.Clamp (weaponSelect, 0, 1);
			
			ActiveWeapon ();
		}

		/// <summary>
		/// Активация оружия
		/// </summary>
		void ActiveWeapon () {
			//Первого оружия
			if (weaponSelect == 0 && weaponFirst != null) {
				if (weaponSecond != null) {
					weaponSecond.node.Enabled = false;
					weaponSecond.objectWeapon.Enabled = false;
				}
				
				weaponFirst.node.Enabled = true;
				weaponFirst.objectWeapon.Enabled = true;
			}
			//Второе оружия
			else if (weaponSelect == 1 && weaponSecond != null) {
				if (weaponFirst != null) {
					weaponFirst.node.Enabled = false;
					weaponFirst.objectWeapon.Enabled = false;
				}
				
				weaponSecond.node.Enabled = true;
				weaponSecond.objectWeapon.Enabled = true;
			} else weaponSelect = -1;
			//Активация строителя
			if (weaponSecond != null) {
				if (weaponSecond.objectWeapon.Enabled) weaponSecond.objectWeapon.WeaponBuilder_Active (true);
				else weaponSecond.objectWeapon.WeaponBuilder_Active (false);
			}
		}

		/// <summary>
		/// Активация гранаты
		/// </summary>
		/// <param name="active">
		/// Активна?
		/// </param>
		void SelectGrenade (bool active) {
			if (weaponGrenade.Count != 0 && active)
				weaponGrenade [0].objectWeapon.WeaponGrenade_Spawn (spawnPoint.WorldPosition);
		}
	}
}