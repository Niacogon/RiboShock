using Unigine;

/// <summary>
/// Раздел систем
/// </summary>
namespace RiboShock.Systems {
	/// <summary>
	/// Поворот объекта
	/// </summary>
	[Component (PropertyGuid = "12eae39c619497ecc2a48e93417d3b53a075c817")]
	public class System_ObjectRotation : Component {
		[Parameter (Title = "Множитель", Tooltip = "Умножается ось для поворотц\n0 - нет поворота по оси")]
		public vec3 rotationMultiply = new vec3 (0, 0, 1);
		[Parameter (Title = "Скорость поворота")]
		public float rotationSpeed = 1f;
		
		void Update () {
			node.Rotate (rotationSpeed * rotationMultiply.x, rotationSpeed * rotationMultiply.y, rotationSpeed * rotationMultiply.z);
		}
	}
}