using RiboShock.Systems;
using Unigine;

/// <summary>
/// Раздел котроллеров
/// </summary>
namespace RiboShock.Controller {
	/// <summary>
	/// Контроллер камеры
	/// </summary>
	[Component (PropertyGuid = "b155b51794a40e3f30df607b15af7bef4740529f")]
	public class Controller_Camera : Component {
		[ShowInEditor, Parameter (Title = "Камера пустышка")] PlayerDummy camera = new PlayerDummy();

		[ShowInEditor, Parameter (Group = "Основные настройки", Title = "Дистанция камеры")]
		float cameraDistance = 0;
		[ShowInEditor, Parameter (Group = "Основные настройки", Title = "Угол возвышения", Tooltip = "x - минимальная, y - максимальная, z - стартовая")]
		vec3 elevationAngle = new vec3 (-89.9f, 89.9f, 0);
		[ShowInEditor, ParameterSlider (Group = "Основные настройки", Min = 0.0f, Max = 360.0f, Title = "Начальный азимутальный угол")]
		float initcurAzimuthAngle = 60.0f;
		[ShowInEditor, ParameterSlider (Group = "Основные настройки", Min = 0.0f, Title = "Скорость смещения угла")]
		float turningSpeed = 0.2f;

		[ShowInEditor, Parameter (Group = "Дополнительные настройки", Title = "Фиксированные углы камеры")]
		bool isFixedAngles = false;

		/// <summary>
		/// Точки взгялда
		/// </summary>
		public Node targetsPoint;
		
		/// <summary>
		/// Последняя позиция камеры
		/// </summary>
		vec3 lastTargetPosition = vec3.ZERO;

		/// <summary>
		/// Текущий азимут угла
		/// </summary>
		float curAzimuthAngle = 0;
		/// <summary>
		/// Текущий угол возвыщения
		/// </summary>
		float curElevationAngle = 0;

		void Init () {
			System_Inputs.OnChangeMouseAxis_Event += SetCameraAngles;

			lastTargetPosition = targetsPoint.WorldPosition;
			//Стартовое напровление и позиция камеры
			vec3 _dir = camera.WorldPosition - lastTargetPosition;
			curAzimuthAngle = initcurAzimuthAngle;
			curElevationAngle = elevationAngle.z;

			_dir = new quat (vec3.UP, curAzimuthAngle) * vec3.FORWARD;
			_dir = new quat (MathLib.Cross (vec3.UP, _dir), -curElevationAngle) * _dir;

			if (_dir.Length2 > 0) _dir.Normalize ();

			//Установка позиции и напровление камеры
			camera.WorldPosition = lastTargetPosition + _dir * cameraDistance;
			camera.ViewDirection = -_dir;
		}
		
		void ReInit (Object value) => Init ();

		void Shutdown () {
			System_Inputs.OnChangeMouseAxis_Event -= SetCameraAngles;
		}

		void Update () {
			lastTargetPosition = targetsPoint.WorldPosition;
			//Обновление направления камеры
			vec3 _dir = new quat (vec3.UP, curAzimuthAngle) * vec3.FORWARD;
			_dir = new quat (MathLib.Cross (vec3.UP, _dir), -curElevationAngle) * _dir;
			if (_dir.Length2 > 0) _dir.Normalize ();

			//Установка позиции и направление камеры относительно цели
			camera.WorldPosition = lastTargetPosition + _dir * cameraDistance;
			camera.ViewDirection = -_dir;
		}

		/// <summary>
		/// Установка углов камеры
		/// </summary>
		/// <param name="mouseAxis">
		/// Ось мыши
		/// </param>
		void SetCameraAngles (vec2 mouseAxis) {
			if (!System_Inputs.mouseGrab) return;

			if (!isFixedAngles && Game.Frame != 1) {
				curAzimuthAngle -= mouseAxis.x * turningSpeed;
				curElevationAngle = MathLib.Clamp (curElevationAngle + mouseAxis.y * turningSpeed, elevationAngle.x, elevationAngle.y);
			}
		}
	}
}