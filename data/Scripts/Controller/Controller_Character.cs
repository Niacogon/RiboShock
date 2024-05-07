using Unigine;
using RiboShock.Creator;
using RiboShock.Systems;

/// <summary>
/// Раздел котроллеров
/// </summary>
namespace RiboShock.Controller {
	/// <summary>
	/// Контроллер персонажа
	/// </summary>
	[Component (PropertyGuid = "c23743caa24e100f61a96976a363db0fbdcc058e")]
	public class Controller_Character : Component {
		[Parameter (Group = "Настройки персонажа", Title = "Создатель массы тела")]
		public Creator_BodyRigid bodyRigid = new Creator_BodyRigid ();

		[ShowInEditor, Parameter (Group = "Настройки персонажа", Title = "Направление от цели", Tooltip = "К примеру камера или цель")]
		Node targetDirection = null;
		[ShowInEditor, Parameter (Group = "Настройки персонажа", Title = "Движение по направлению")]
		bool lookAtDirection = true;
		[ShowInEditor, Parameter (Group = "Настройки персонажа", Title = "Параметры прыжка")]
		float jumping = 1.2f;
		/// <summary>
		///	Состояний поведения персонажа
		/// </summary>
		Controller_Character_BehaviorState behaviorStates = new Controller_Character_BehaviorState ();

		ObjectDummy objectDummy = null;
		/// <summary>
		/// Фичисеское тело персонажа
		/// </summary>
		public BodyRigid rigid { private set; get; } = null;
		ShapeCapsule shape = null;

		vec3 velocity = vec3.ZERO;
		vec3 position = vec3.ZERO;
		vec3 direction = vec3.ZERO;
		float azimuthAngle = 0f;
		vec3 oldImpulsY = vec3.ZERO;

		/// <summary>
		/// На земле
		/// </summary>
		public int ground { get; private set; } = 0;
		/// <summary>
		/// Потолок
		/// </summary>
		public int ceiling { get; private set; } = 0;

		/*[ShowInEditor, Parameter (Group = "Характеристики", Tooltip = "Контроллер атаки")]
		FireController fireController = null;*/
		[ShowInEditor, ParameterFile (Group = "Эффекты", Tooltip = "Ссылка эффект смерти")]
		string deadEffectReference = null;

		void Init () {
			//Manager_Games_Data.character = this;
			//
			objectDummy = new ObjectDummy ();
			rigid = bodyRigid.SettingBodyRigid ();
			rigid.Name = node.Name + "_rb";
			objectDummy.Body = rigid;
			shape = bodyRigid.SettingShapeCapsule ();
			bodyRigid.SetCollisionParameters ();

			UpdateTransform ();
			//Запуск статуса
			for (int _i = 0; _i < (int) Controller_Character_BehaviorState.Behavior.NumStates; _i++) {
				behaviorStates.states [_i] = 0;
				behaviorStates.times [_i] = 0.0f;
			}

			position = vec3.ZERO;
			direction = new vec3 (1.0f, 0.0f, 0.0f);
			azimuthAngle = 90.0f;

			ground = 0;
			ceiling = 0;

			System_Inputs.OnChangeMoveAxis_Event += UpdateMoveStates;
			System_Inputs.OnChangeJump_Event += UpdateJumpState;
		}

		void Shutdown () {
			System_Inputs.OnChangeMoveAxis_Event -= UpdateMoveStates;
			System_Inputs.OnChangeJump_Event -= UpdateJumpState;
		}
	
		void Update () {
			if (rigid != null) UpdateRigid (Game.IFps);
		}

		/// <summary>
		///	Реализация события смерти
		/// </summary>
		void DeadEventHandler () {
			System_Inputs.SetMouseGrab (false);

			Node effect = World.LoadNode (deadEffectReference);
			effect.WorldPosition = node.WorldPosition;

			node.DeleteLater();
		}

		/// <summary>
		/// Обновление статуса положения
		/// </summary>
		/// <param name="condition">
		/// Состояние
		/// </param>
		/// <param name="state">
		/// Номер состояния
		/// </param>
		/// <param name="begin">
		/// Запуск состояния (Как целое число, где 0 = не начинать)
		/// </param>
		/// <param name="end">
		/// Конец состояния (Как целое число, где 0 = не заканчивать)
		/// </param>
		/// <param name="ifps">
		/// Частота обновления
		/// </param>
		int UpdateState (bool condition, int state, int begin, int end, float ifps) {
			//переключение статуса с Diasbled на Begin
			if (condition && behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.Diasbled && begin == 1) {
				behaviorStates.states [state] = (int) Controller_Character_BehaviorState.StateStatus.Begin;
				behaviorStates.times [state] = 0.0f;
				return (int) Controller_Character_BehaviorState.StateStatus.Begin;
			}

			//переключение статуса с Enabled или Begin на End
			if (!condition && (behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.Enabled
			|| behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.Begin) && end == 1) {
				behaviorStates.states [state] = (int) Controller_Character_BehaviorState.StateStatus.End;
				return (int) Controller_Character_BehaviorState.StateStatus.End;
			}

			//переключение статуса с Begin на Enabled
			if ((condition && behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.Begin)
			|| behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.Enabled) {
				behaviorStates.states [state] = (int) Controller_Character_BehaviorState.StateStatus.Enabled;
				behaviorStates.times [state] += ifps;
				return (int) Controller_Character_BehaviorState.StateStatus.Enabled;
			}

			//переключение статуса с End на Disabled
			if (behaviorStates.states [state] == (int) Controller_Character_BehaviorState.StateStatus.End) {
				behaviorStates.states [state] = (int) Controller_Character_BehaviorState.StateStatus.Diasbled;
				return (int) Controller_Character_BehaviorState.StateStatus.Diasbled;
			}

			return (int) Controller_Character_BehaviorState.StateStatus.Diasbled;
		}

		/// <summary>
		/// Обновление состояний движения
		/// </summary>
		/// <param name="moveAxis">
		/// Ось движения
		/// </param>
		void UpdateMoveStates (vec3 moveAxis) {
			UpdateState (moveAxis.y > 0, (int) Controller_Character_BehaviorState.Behavior.Forward, 1, 1, Game.IFps);
			UpdateState (moveAxis.y < 0, (int) Controller_Character_BehaviorState.Behavior.Backward, 1, 1, Game.IFps);

			UpdateState (moveAxis.x < 0, (int) Controller_Character_BehaviorState.Behavior.Left, 1, 1, Game.IFps);
			UpdateState (moveAxis.x > 0, (int) Controller_Character_BehaviorState.Behavior.Right, 1, 1, Game.IFps);

			UpdateState (moveAxis.z == 0, (int) Controller_Character_BehaviorState.Behavior.Crouch, 1, 1, Game.IFps);
			UpdateState (moveAxis.z == 1, (int) Controller_Character_BehaviorState.Behavior.Walk, 1, 1, Game.IFps);
			UpdateState (moveAxis.z == 2, (int) Controller_Character_BehaviorState.Behavior.Run, 1, 1, Game.IFps);
			UpdateState (moveAxis.z == 3, (int) Controller_Character_BehaviorState.Behavior.Sprint, 1, 1, Game.IFps);
		}

		/// <summary>
		/// Обновление состояние прыжка
		/// </summary>
		/// <param name="onJump">
		/// Прыгаем?
		/// </param>
		void UpdateJumpState (bool onJump) {
			UpdateState (onJump, (int) Controller_Character_BehaviorState.Behavior.Jump, ground, 1, Game.IFps);
		}

		/// <summary>
		/// Обновление состояний
		/// </summary>
		/// <param name="enabled">
		/// Активен? Как целое число, где 0 = не активен
		/// </param>
		/// <param name="ifps">
		/// Частота обновления
		/// </param>
		void UpdateStates (int enabled, float ifps) {
			if (enabled == 1 && !Unigine.Console.Active) {
				//обработка системы ввода
				//UpdateState ((Input.IsKeyPressed (fireKey) || (Input.MouseGrab && !waitClickAfterGrab && Input.IsMouseButtonPressed (mouseFireKey))), (int) State.Fire, 1, 1, ifps);
			} else {
				//отключение состояний
				for (int _i = 0; _i < (int) Controller_Character_BehaviorState.Behavior.NumStates; _i++) UpdateState (false, _i, 1, 1, ifps);
			}
		}

		/// <summary>
		/// Обновление контроллера (физическая масса)
		/// </summary>
		/// <param name="ifps">
		/// Частота обновления
		/// </param>
		void UpdateRigid (float ifps) {
			vec3 _up = vec3.UP;
			vec3 _impulse = vec3.ZERO;

			vec3 _y = new quat (_up, -azimuthAngle) * vec3.FORWARD;
			vec3 _x = MathLib.Normalize (MathLib.Cross (_y, _up));
			vec3 _z = MathLib.Normalize (MathLib.Cross (_x, _y));

			UpdateStates (1, ifps);

			//старая скорость
			float _xVelocity = MathLib.Dot (_x, rigid.LinearVelocity);
			float _yVelocity = MathLib.Dot (_y, rigid.LinearVelocity);
			float _zVelocity = MathLib.Dot (_z, rigid.LinearVelocity);

			//напровление
			vec3 _playerDir = targetDirection.GetWorldDirection (MathLib.AXIS.NZ);
			_playerDir.z = 0;
			_playerDir.Normalize ();

			azimuthAngle = MathLib.Angle (_playerDir, vec3.FORWARD, vec3.UP);

			//новая база
			_y = new quat (_up, -azimuthAngle) * vec3.FORWARD;
			_x = MathLib.Normalize (MathLib.Cross (_y, _up));
			_z = MathLib.Normalize (MathLib.Cross (_x, _y));

			//движение
			//if (ground == 1) {
				oldImpulsY = vec3.ZERO;

				if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Forward] > 0) {
					_impulse += _y;
					oldImpulsY = _y;
				}
				
				if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Backward] > 0) _impulse -= _y;
				if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Left] > 0) _impulse -= _x;
				if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Right] > 0) _impulse += _x;
			//} else _impulse += oldImpulsY;

			if (_impulse.Length2 > 0) _impulse.Normalize ();

			//разморозка массы тела
			if (_impulse.Length2 > MathLib.EPSILON) rigid.Frozen = false;

			//атака
			/*if (states[(int)State.Fire] > 0)
				fireController.Fire();*/

			//взгляд по направлению
			if (_impulse.Length2 > 0 && lookAtDirection) {
				direction = MathLib.Lerp (direction, _playerDir, 7.5f * ifps);
				if (direction.Length2 > 0) direction.Normalize ();
			}

			//скорость
			if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Crouch] > 0) _impulse *= bodyRigid.m_velocity.x;
			if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Walk] > 0) _impulse *= bodyRigid.m_velocity.x;
			if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Run] > 0) _impulse *= bodyRigid.m_velocity.y;
			if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Sprint] > 0)  _impulse *= bodyRigid.m_velocity.z;

			//прыжок
			if (behaviorStates.states [(int) Controller_Character_BehaviorState.Behavior.Jump] == (int) Controller_Character_BehaviorState.StateStatus.Begin) {
				rigid.Frozen = false;
				_impulse += _z * MathLib.Sqrt (2.0f * 9.8f * jumping) / (bodyRigid.m_acceleration * ifps);
			}

			//скорость поворота
			if (ground > 0) rigid.LinearVelocity = _x * _xVelocity + _y * _yVelocity + _z * _zVelocity;

			//целевая скорость
			float _targetVelocity = MathLib.Length (new vec2 (MathLib.Dot (_x, _impulse), MathLib.Dot (_y, _impulse)));

			//сохранение старой скорости
			velocity = rigid.LinearVelocity;
			float _oldVelocity = MathLib.Length (new vec2 (MathLib.Dot (_x, velocity), MathLib.Dot (_y, velocity)));

			//интеграция скорости
			rigid.AddLinearImpulse (_impulse * (bodyRigid.m_acceleration * ifps * shape.Mass));

			//затухание
			float _currentVelocity = MathLib.Length (new vec2 (MathLib.Dot (_x, rigid.LinearVelocity), MathLib.Dot (_y, rigid.LinearVelocity)));
			if (_targetVelocity < MathLib.EPSILON || _currentVelocity > _targetVelocity) {
				vec3 _linearVelocity = _z * MathLib.Dot (_z, rigid.LinearVelocity);
				_linearVelocity += (_x * MathLib.Dot (_x, rigid.LinearVelocity) + _y * MathLib.Dot (_y, rigid.LinearVelocity)) * MathLib.Exp (-bodyRigid.m_damping * ifps);
				rigid.LinearVelocity = _linearVelocity;
			}

			//фиксация максимальной скорости
			_currentVelocity = MathLib.Length (new vec2 (MathLib.Dot (_x, rigid.LinearVelocity), MathLib.Dot (_y, rigid.LinearVelocity)));
			if (_currentVelocity > _oldVelocity) {
				if (_currentVelocity > _targetVelocity) {
					vec3 linearVelocity = _z * MathLib.Dot (_z, rigid.LinearVelocity);
					linearVelocity += (_x * MathLib.Dot (_x, rigid.LinearVelocity) + _y * MathLib.Dot (_y, rigid.LinearVelocity)) * _targetVelocity / _currentVelocity;
					rigid.LinearVelocity = linearVelocity;
				}
			}

			//замороска скорости
			if (_currentVelocity < MathLib.EPSILON) rigid.LinearVelocity = _z * MathLib.Dot (_z, rigid.LinearVelocity);

			// friction
			if (_targetVelocity < MathLib.EPSILON) shape.Friction = bodyRigid.m_friction.y;
			else shape.Friction = bodyRigid.m_friction.x;

			//убрираем флаги столкновения
			ground = 0;
			ceiling = 0;

			//получения стокновения
			vec3 _cap0 = shape.BottomCap;
			vec3 _cap1 = shape.TopCap;
			for (int i = 0; i < rigid.NumContacts; i++) {
				vec3 _point = rigid.GetContactPoint (i);
				vec3 _normal = rigid.GetContactNormal (i);
				if (MathLib.Dot (_normal, _up) > 0.5f && MathLib.Dot (new vec3 (_point - _cap0), _up) < 0.0f)
					ground = 1;
				if (MathLib.Dot (_normal, _up) < -0.5f && MathLib.Dot (new vec3 (_point - _cap1), _up) > 0.0f)
					ceiling = 1;
			}

			//установка текущих параметров
			position = objectDummy.WorldTransform.GetColumn3 (3);
			FlushTransform ();
		}

		/// <summary>
		/// Обновление трансофрма
		/// </summary>
		void UpdateTransform () {
			vec3 _up = vec3.UP;
			vec3 _tangent, _binormal;
			MathLib.OrthoBasis (_up, out _tangent, out _binormal);

			//декомпресия трансорма
			position = node.WorldTransform.GetColumn3 (3);
			direction = MathLib.Normalize (new vec3 (node.WorldTransform.GetColumn3 (1)));

			//декомпресия направления
			azimuthAngle = MathLib.Atan2 (MathLib.Dot (direction, _tangent), MathLib.Dot (direction, _binormal)) * MathLib.RAD2DEG;
			objectDummy.WorldTransform = GetBodyTransform ();
		}

		/// <summary>
		/// Освежить трансформ
		/// </summary>
		void FlushTransform () {
			node.WorldTransform = MathLib.SetTo (position, position + direction, vec3.UP, MathLib.AXIS.Y);
		}

		#region Privet voids
		/// <summary>
		/// Трансформация игрока
		/// </summary>
		mat4 GetBodyTransform () {
			vec3 _up = vec3.UP;
			vec3 _center = position;
			return MathLib.SetTo (_center, _center + new vec3 (direction - _up * MathLib.Dot (direction, _up)), _up) * new mat4 (MathLib.RotateX (-90.0f) * MathLib.RotateZ (90.0f));
		}

		mat4 GetModelview () {
			vec3 _up = vec3.UP;
			vec3 _eye = position + new vec3 (_up * (shape.Height + shape.Radius));
			return MathLib.LookAt (_eye, _eye + new vec3 (direction), _up);
		}
		#endregion

		#region Public voids
		/// <summary>
		/// Принудительная трансформа игрока
		/// </summary>
		/// <param name="transform">
		/// Данные трансформа
		/// </param>
		public void SetTransform (mat4 transform) {
			node.Transform = transform;
			UpdateTransform ();
		}

		/// <summary>
		/// Принудительная установка позиции игрока
		/// </summary>
		/// <param name="transform">
		/// Данные трансформа
		/// </param>
		public void SetWorldTransform (mat4 transform) {
			node.WorldTransform = transform;
			UpdateTransform ();
		}

		/// <summary>
		/// Принудительная установка азимута угла
		/// </summary>
		/// <param name="angle">
		/// Угол
		/// </param>
		public void SetAzimuthAngle (float angle) {
			angle = angle - azimuthAngle;
			direction = new quat (vec3.UP, angle) * direction;
			azimuthAngle += angle;

			FlushTransform ();
		}

		/// <summary>
		/// Запром азимута угла
		/// </summary>
		public float GetPhiAngle () {
			return azimuthAngle;
		}

		/// <summary>
		/// Принудительная установка напровления
		/// </summary>
		/// <param name="view">
		/// Взгляд
		/// </param>
		public void SetViewDirection (vec3 view) {
			direction = MathLib.Normalize (view);

			vec3 _tangent, _binormal;
			MathLib.OrthoBasis (vec3.UP, out _tangent, out _binormal);
			azimuthAngle = MathLib.Atan2 (MathLib.Dot (direction, _tangent), MathLib.Dot (direction, _binormal)) * MathLib.RAD2DEG;

			FlushTransform ();
		}
		#endregion
	}
}