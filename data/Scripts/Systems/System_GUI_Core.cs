using System.Reflection.Metadata;
using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Ядро пользовательского интерфейса
	/// </summary>
	[Component (PropertyGuid = "19a98c3a54473992ed5bfbb41ba488c6910440a1")]
	public class System_GUI_Core : Component {
		/// <summary>
		/// Графический пользовательский интерфейс
		/// </summary>
		public Gui gui { private set; get; } = Gui.GetCurrent ();
		/// <summary>
		/// Основной холст
		/// </summary>
		public WidgetCanvas mainCanvas { private set; get; }

		[Parameter (Title = "Другие настройки")]
		public bool widgetCustomSettings = false;

		[ParameterCondition (nameof (widgetCustomSettings), 1), Parameter (Title = "Пивот по центру")]
		public bool widgetCentrPivot = false;
		[ParameterCondition (nameof (widgetCustomSettings), 1), Parameter (Title = "Позиция виджета", Tooltip = "Позиция выставляет относительно разрешения\n" +
			 "в процентном соотношении\nX - ширина, Y - высота")]
		public vec2 widgetPosition = vec2.ZERO;
		[ParameterCondition (nameof (widgetCustomSettings), 1), Parameter (Title = "Смещение виджета", Tooltip = "На сколько будет отклоняться виджет от позиции\n" +
			 "в пикселях\nX - ширина, Y - высота")]
		public vec2 widgetOffsetPosition = vec2.ZERO;
		[ParameterCondition (nameof (widgetCustomSettings), 1), ParameterColor (Title = "Цвет виджета", Tooltip = "Формат RGBA")]
		public vec4 widgetColor = vec4.WHITE;

		void Init () {
			//Создание основного холста для UI
			mainCanvas = new WidgetCanvas ();
			mainCanvas.Width = gui.Width;
			mainCanvas.Height = gui.Height;
			gui.AddChild (mainCanvas, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
			//Создание виджетов
			WidgetInit ();
		}
		
		/// <summary>
		/// Создание виджета
		/// </summary>
		public virtual void WidgetInit () {}
	}
}