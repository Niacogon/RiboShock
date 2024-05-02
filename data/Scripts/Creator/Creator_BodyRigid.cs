using System;
using Unigine;

/// <summary>
/// Раздел создателей
/// </summary>
namespace RiboShock.Creator {
	/// <summary>
	/// Физическое тело - Контроллер
	/// </summary>
	[Serializable, Component (PropertyGuid = "43e27d7f6cbf3a52d853076331bf6e186ef59e07")]
	public class Creator_BodyRigid {
		/// <summary>
		/// Масса физического тела
		/// </summary>
		BodyRigid rigid = new BodyRigid ();
		/// <summary>
		/// Сферический коллайдер
		/// </summary>
		ShapeCapsule shape = new ShapeCapsule ();

		[ShowInEditor, Parameter (Group = "Масса физического тела", Title = "Физическая масса")]
		float physicalMass = 30.0f;
		
		[ShowInEditor, Parameter (Group = "Масса физического тела", Title = "Скорость", Tooltip = "x - шаг/ползти, y - бег, z - спринт)")]
		vec3 velocity = new vec3 (1.5f, 3.4f, 5.8f);
		
		/// <summary>
		/// Скорость (x - минимальная, y - максимальная)
		/// </summary>
		public vec3 m_velocity => velocity;
		
		[ShowInEditor, Parameter (Group = "Масса физического тела", Title = "Ускорение")]
		float acceleration = 8.0f;
		
		/// <summary>
		/// Ускорение
		/// </summary>
		public float m_acceleration => acceleration;
		[ShowInEditor, Parameter (Group = "Масса физического тела", Title = "Затухание")]
		float damping = 8.0f;
		/// <summary>
		/// Затухание
		/// </summary>
		public float m_damping => damping;
		[ShowInEditor, ParameterMask (Group = "Масса физического тела", Title = "Физическая маска")]
		int physicalMask = 1;

		[ShowInEditor, Parameter (Group = "Сферический коллайдер", Title = "Высота")]
		float height = 0.8f;
		[ShowInEditor, Parameter (Group = "Сферический коллайдер", Title = "Радиус")]
		float radius = .5f;
		[ShowInEditor, Parameter (Group = "Сферический коллайдер", Title = "Трение", Tooltip = "x - минимальное, y - максимальное")]
		vec2 friction = new vec2 (.1f, 1f);
		
		/// <summary>
		/// Трение (x - минимальное, y - максимальное)
		/// </summary>
		public vec2 m_friction => friction;
		
		[ShowInEditor, ParameterMask (Group = "Сферический коллайдер", Title = "Маска колизии")]
		int collisionMask = 1;
		[ShowInEditor, ParameterMask (Group = "Сферический коллайдер", Title = "Маска пересечения")]
		int intersectionMask = 1;

		/// <summary>
		/// Установка физического тела
		/// </summary>
		/// <returns>
		/// Физическое тело
		/// </returns>
		public BodyRigid SettingBodyRigid () {
			rigid = new BodyRigid ();
			rigid.MaxAngularVelocity = 0.0f;
			rigid.Freezable = true;
			rigid.Enabled = true;

			rigid.PhysicalMask = physicalMask;

			return rigid;
		}

		/// <summary>
		/// Установка сферический коллайдер
		/// </summary>
		/// <returns>
		/// Сферический коллайдер
		/// </returns>
		public ShapeCapsule SettingShapeCapsule () {
			shape = new ShapeCapsule (radius, height);
			shape.Restitution = 0.0f;
			shape.Continuous = false;
			shape.Body = rigid;

			shape.Mass = physicalMass;
			shape.PhysicsIntersectionMask = intersectionMask;
			shape.CollisionMask = collisionMask;

			return shape;
		}

		/// <summary>
		/// Установка параметров радиуса и высоты сферического коллайдера
		/// </summary>
		public void SetCollisionParameters () {
			vec3 _up = vec3.UP;
			//радиус
			if (!MathLib.Equals (shape.Radius, radius)) {
				rigid.SetPreserveTransform (new mat4 (MathLib.Translate (_up * (radius - shape.Radius))) * rigid.Transform);
				shape.Radius = radius;
			}
			//высота
			if (!MathLib.Equals (shape.Height, height)) {
				rigid.SetPreserveTransform (new mat4 (MathLib.Translate (_up * (height - shape.Height) * 0.5f)) * rigid.Transform);
				shape.Height = height;
			}
		}

		/// <summary>
		/// Запроос радиуса сферического коллайдера
		/// </summary>
		/// <returns>
		/// Радиус коллайдера (float)
		/// </returns>
		public float GetCollisionRadius () {
			return shape.Radius;
		}

		/// <summary>
		/// Запроос высоты сферического коллайдера
		/// </summary>
		/// <returns>
		/// Высота коллайдера (float)
		/// </returns>
		public float GetCollisionHeight () {
			return shape.Height;
		}
	}
}