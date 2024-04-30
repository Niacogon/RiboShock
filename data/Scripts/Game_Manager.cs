using Unigine;

/// <summary>
/// Основной раздел
/// </summary>
namespace RiboShock {
	/// <summary>
	/// Игровой менеджер
	/// </summary>
	[Component (PropertyGuid = "57eba3dc6afedf451ddcf06c50374d05d7e748bf")]
	public class Game_Manager : Component {
		void Init () {
			Unigine.Console.Onscreen = true;
		}
		
		void Shutdown () {
			Unigine.Console.Onscreen = false;
		}
	}
}