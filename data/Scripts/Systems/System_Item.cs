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
		[ShowInEditor, Parameter (Title = "Имя предмета")]
		private string itemName = "";
		/// <summary>
		/// Запрос имени предмета
		/// </summary>
		/// <returns></returns>
		public string GetItemName () {
			return itemName;
		}
	}
}