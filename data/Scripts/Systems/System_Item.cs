using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Предмет
	/// </summary>
	[Component (PropertyGuid = "3e63fd7363fc488ffd8cbe23907da8d846e3aa9a")]
	public class System_Item : Component {
		[Parameter (Title = "Поворот предмета")]
		public System_ObjectRotation objectRotation;
		[Parameter (Title = "Оружие")]
		public System_Weapon objectWeapon;
		
		/// <summary>
		/// Физические свойства
		/// </summary>
		public Body body;

		[ShowInEditor, Parameter (Title = "Имя предмета")]
		private string itemName = "";
		/// <summary>
		/// Запрос имени предмета
		/// </summary>
		/// <returns></returns>
		public string GetItemName () {
			return itemName;
		}

		[ShowInEditor, Parameter (Title = "ID предмета")]
		private int itemId = 0;
		/// <summary>
		/// Запрос ID предмета
		/// </summary>
		/// <returns></returns>
		public int GetItemID () {
			return itemId;
		}

		[ShowInEditor, ParameterSwitch (Title = "Тип предмета", Items = "Первое оружие,Второе оружие")]
		private int itemType = 0;
		/// <summary>
		/// Запрос типа оружия (0 = первое оржуие; 1 = второе оружие)
		/// </summary>
		/// <returns></returns>
		public int GetTypeItem () {
			return itemType;
		}

		void Init () {
			body = node.ObjectBody;
		}
	}
}