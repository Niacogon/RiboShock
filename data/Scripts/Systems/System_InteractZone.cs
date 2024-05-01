using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Зона взаимодействия
	/// </summary>
	[Component (PropertyGuid = "7afcd4924f248e54ece8a8a2d8c94fa4a77913dd")]
	public class System_InteractZone : Component {
		[ShowInEditor, Parameter (Title = "Физический триггер")]
		private PhysicalTrigger physicalTrigger;

		private System_Item currentItem;

		public delegate void ChangeInteractionObject (string name, bool active);
		public event ChangeInteractionObject OnChangeInteractionObject_Event;
		
		void Init () {
			physicalTrigger.EventEnter.Connect (EnteringBody);
			physicalTrigger.EventLeave.Connect (LeavingBody);
		}
		
		void Shutdown () {
			physicalTrigger.EventEnter.Disconnect (EnteringBody);
			physicalTrigger.EventLeave.Disconnect (LeavingBody);
		}
		
		/// <summary>
		/// Событие входа объекта в зону
		/// </summary>
		/// <param name="enteringBody">
		/// Входящий объект
		/// </param>
		void EnteringBody (Body enteringBody) {
			//Предметы
			if (enteringBody.Object.GetComponent <System_Item> ()) {
				currentItem = enteringBody.Object.GetComponent <System_Item> ();
				OnChangeInteractionObject_Event?.Invoke (currentItem.GetItemName (), true);
			}
		}
		
		/// <summary>
		/// Событие входа объекта в зону
		/// </summary>
		/// <param name="leavingBody">
		/// Выходящий объект
		/// </param>
		void LeavingBody (Body leavingBody) {
			//Предметы
			if (leavingBody.Object.GetComponent <System_Item> () && currentItem == leavingBody.Object.GetComponent <System_Item> ()) {
				OnChangeInteractionObject_Event?.Invoke (currentItem.GetItemName (), false);
			}
		}
	}
}