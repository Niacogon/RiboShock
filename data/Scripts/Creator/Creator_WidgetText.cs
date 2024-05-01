using Unigine;
using RiboShock.Systems;

/// <summary>
/// Раздел создателей
/// </summary>
namespace RiboShock.Creator {
	/// <summary>
	/// Виджет для текста - GUI
	/// </summary>
	[Component (PropertyGuid = "57f1a9a97eee3e3ddc4a7387020fb9b4f905f29a")]
	public class Creator_WidgetText : System_GUI_Core {
		/// <summary>
		/// Виджет текста
		/// </summary>
		public WidgetLabel widgetLabel { private set; get; }

		[ShowInEditor, Parameter (Title = "Текст виджета")]
		private string widgetText = "";
		[Parameter (Title = "Размер текста виджета")]
		public int widgetTextSize = 20;
		[Parameter (Title = "Обводка текста")]
		public bool widgetTextOutline = false;

		[Parameter (Title = "Виджет активен")]
		public bool widgetActive = true;
		
		/// <summary>
		/// Создание виджета
		/// </summary>
		public override void WidgetInit () {
			widgetLabel = new WidgetLabel ();
			widgetLabel.Text = widgetText;
			widgetLabel.FontSize = widgetTextSize;
			widgetLabel.FontOutline = widgetTextOutline ? 1 : 0;
			widgetLabel.FontColor = widgetColor;
			
			widgetLabel.Arrange ();
			widgetLabel.Hidden = !widgetActive;
			gui.AddChild (widgetLabel, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
			//Позиция
			vec2 _targetPos = new vec2 (gui.Width * widgetPosition.x, gui.Height * widgetPosition.y);
			_targetPos += widgetOffsetPosition;

			if (widgetCentrPivot) {
				_targetPos.x -= widgetLabel.Width / 2;
				_targetPos.y -= widgetLabel.Height / 2;
			}
			
			widgetLabel.SetPosition ((int) _targetPos.x, (int) _targetPos.y);
		}
	}
}