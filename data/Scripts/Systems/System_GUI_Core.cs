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
		[ParameterCondition (nameof (widgetCustomSettings), 1), Parameter (Title = "Отступ", Tooltip = "Установка отступов внутри виджета в пикселях\n" +
			 "X - ширина, Y - высота")]
		public ivec2 widgetSpace = new ivec2 (5, 5);
		[ParameterCondition (nameof (widgetCustomSettings), 1), Parameter (Title = "Пробелы", Tooltip = "Установка прорбелов внутри виджета в пикселях\n" +
			 "X - левый, Y - правый, Z - вверхний, W - нижний")]
		public ivec4 widgetPadding = new ivec4 (5, 10, 5, 5);

		#region Interaction
		[Parameter (Group = "Панель взаимодействия", Title = "Панель взаимодействия")]
		public WidgetGridBox interactionPanel;
		[Parameter (Group = "Панель взаимодействия", Title = "Текст взаимодействия")]
		public WidgetLabel interactionText;
		[Parameter (Group = "Панель взаимодействия", Title = "Описание взаимодействия")]
		public WidgetLabel interactionDescription;

		[Parameter (Group = "Панель взаимодействия", Title = "Триггер взаимодействия")]
		public System_InteractZone interactionZone;
		[ParameterColor (Group = "Панель взаимодействия", Title = "Цвет заднего фона")]
		public vec4 interactionColor = vec4.MAGENTA;
		[Parameter (Group = "Панель взаимодействия", Title = "Размер имени объекта")]
		public int interactionNameSize = 25;
		[Parameter (Group = "Панель взаимодействия", Title = "Размер описания")]
		public int interactionDescriptionSize = 20;
		#endregion

		void Init () {
			//Создание основного холста для UI
			mainCanvas = new WidgetCanvas ();
			mainCanvas.Width = gui.Width;
			mainCanvas.Height = gui.Height;
			gui.AddChild (mainCanvas, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP);
			//Создание виджетов
			WidgetInit ();
			InteractionPanel_Init ();
		}
		
		void Shutdown () {
			//Одписка
			interactionZone.OnChangeInteractionObject_Event -= ChangeInteractionText;
		}
		
		/// <summary>
		/// Создание виджета
		/// </summary>
		public virtual void WidgetInit () {}

		/// <summary>
		/// Создание панели воздействия
		/// </summary>
		void InteractionPanel_Init () {
			interactionPanel = new WidgetGridBox ();
			interactionPanel.NumColumns = 1;
			interactionPanel.SetSpace (widgetSpace.x, widgetSpace.y);
			interactionPanel.SetPadding (widgetPadding.x, widgetPadding.y, widgetPadding.z, widgetPadding.w);
			interactionPanel.Background = 1;
			interactionPanel.BackgroundColor = interactionColor;
			//Установка текста
			interactionText = new WidgetLabel ();
			interactionText.Text = "interaction_text";
			interactionText.FontSize = interactionNameSize;
			interactionText.FontOutline = 1;
			interactionText.Arrange ();
			//Установка описания
			interactionDescription = new WidgetLabel ();
			interactionDescription.Text = "[F] поднять";
			interactionDescription.FontSize = interactionDescriptionSize;
			interactionDescription.FontOutline = 1;
			interactionDescription.Arrange ();
			//
			interactionPanel.AddChild (interactionText, Gui.ALIGN_LEFT);
			interactionPanel.AddChild (interactionDescription, Gui.ALIGN_LEFT);
			interactionPanel.Arrange ();
			
			gui.AddChild (interactionPanel, Gui.ALIGN_CENTER);
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
		}
	}
}