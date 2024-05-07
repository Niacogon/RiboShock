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

		void Init () {
			System_Inputs.onSlot += SelectWeapon_Cell;
			System_Inputs.OnChangeMouseWheel_Event += SelectWeapon_Wheel;
		}

		void Shutdown () {
			System_Inputs.onSlot -= SelectWeapon_Cell;
			System_Inputs.OnChangeMouseWheel_Event -= SelectWeapon_Wheel;
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
			//Выбор подоброного оружия
			if (weaponSelect == -1) {
				weaponSelect = newItem.GetTypeItem ();
				ActiveWeapon ();
			}
		}
		
		/// <summary>
		/// Удаление предмета
		/// </summary>
		/// <param name="curItem">
		/// Текущий предмет
		/// </param>
		public void RemoveItem (System_Item curItem) {
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
				if (weaponSecond != null) weaponSecond.node.Enabled = false;
				weaponFirst.node.Enabled = true;
			} else if (weaponSecond == null) weaponSelect = -1;
			//Второе оружия
			if (weaponSelect == 1 && weaponSecond != null) {
				if (weaponFirst != null) weaponFirst.node.Enabled = false;
				weaponSecond.node.Enabled = true;
			} else if (weaponFirst == null) weaponSelect = -1;
		}
	}
}