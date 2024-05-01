using Unigine;
using RiboShock.Systems;

/// <summary>
/// Раздел создателей
/// </summary>
namespace RiboShock.Creator {
	/// <summary>
	/// Подсказка - GUI
	/// </summary>
	[Component (PropertyGuid = "bbef67ab512af6de1f5d7d746af7491ff2ebdaed")]
	public class Creator_Tooltip : System_GUI_Core {
		[Parameter (Group = "Панель взаимодействия", Title = "Панель взаимодействия")]
		public WidgetGridBox interactionPanel;
		[Parameter (Group = "Панель взаимодействия", Title = "Текст взаимодействия")]
		public WidgetLabel interactionText;
		[Parameter (Group = "Панель взаимодействия", Title = "Описание взаимодействия")]
		public WidgetLabel interactionDescription;

		[Parameter (Group = "Панель взаимодействия", Title = "Триггер взаимодействия")]
		public System_InteractZone interactionZone;
		[Parameter (Group = "Панель взаимодействия", Title = "Размер имени объекта")]
		public int interactionNameSize = 25;
		[Parameter (Group = "Панель взаимодействия", Title = "Размер описания")]
		public int interactionDescriptionSize = 20;
		
		void Shutdown () {
			//Одписка
			interactionZone.OnChangeInteractionObject_Event -= ChangeInteractionText;
		}
		
		/// <summary>
		/// Создание виджета
		/// </summary>
		public override void WidgetInit () {
			interactionPanel = new WidgetGridBox ();
			interactionPanel.NumColumns = 1;
			interactionPanel.SetSpace (widgetSpace.x, widgetSpace.y);
			interactionPanel.SetPadding (widgetPadding.x, widgetPadding.y, widgetPadding.z, widgetPadding.w);
			interactionPanel.Background = 1;
			interactionPanel.BackgroundColor = widgetColor;
			//Установка текста
			interactionText = new WidgetLabel ();
			interactionText.Text = "interaction_text";
			interactionText.FontSize = interactionNameSize;
			interactionText.FontOutline = 1;
			interactionText.Arrange ();
			//Установка описания
			interactionDescription = new WidgetLabel ();
			interactionDescription.Text = "[" + Game_Manager.customKeyAssignment_Static.interactionKey + "] поднять";
			interactionDescription.FontSize = interactionDescriptionSize;
			interactionDescription.FontOutline = 1;
			interactionDescription.Arrange ();
			//
			interactionPanel.AddChild (interactionText, Gui.ALIGN_LEFT);
			interactionPanel.AddChild (interactionDescription, Gui.ALIGN_LEFT);
			interactionPanel.Arrange ();
			
			gui.AddChild (interactionPanel, Gui.ALIGN_OVERLAP | Gui.ALIGN_EXPAND);
			interactionPanel.Hidden = true;
			//Подписка
			interactionZone.OnChangeInteractionObject_Event += ChangeInteractionText;
		}
		
		/// <summary>
		/// Изменить текст взаимодействия
		/// </summary>
		/// <param name="nameObject">
		/// Имя объекта
		/// </param>
		/// <param name="active">
		/// Активировать?
		/// </param>
		void ChangeInteractionText (string nameObject, bool active) {
			interactionText.Text = nameObject;
			interactionText.Arrange ();
			
			interactionPanel.Arrange ();
			interactionPanel.Hidden = !active;
			//Обновление позиции
			vec2 _targetPos = new vec2 (gui.Width * widgetPosition.x, gui.Height * widgetPosition.y);
			_targetPos += widgetOffsetPosition;

			if (widgetCentrPivot) {
				_targetPos.x -= interactionPanel.Width / 2;
				_targetPos.y -= interactionPanel.Height / 2;
			}
			
			interactionPanel.SetPosition ((int) _targetPos.x, (int) _targetPos.y);
		}
	}
}