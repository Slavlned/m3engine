using UnityEngine;

// источник звука
public class SoundSource : MonoBehaviour {
	// исходрники
	private AudioSource source;

	// авейк
	private void Awake() {
		// получение компонента
		source = GetComponent<AudioSource>();
		// если звуки включены
		source.Play();
		if (!Settings.IsSoundEnabled) {
			source.Pause();
		}
	}

	// когда настройки изменены
	private void OnSettingsChanged() {
		// если звуки выключены - останавливаем
		if (!Settings.IsSoundEnabled) {
			source.Pause();
		} else {
			// в ином случае - включаем
			source.UnPause();
		}
	}

	// ивенты
	private void OnEnable() {
		Settings.onSettingsChanged += OnSettingsChanged;
	}

	private void OnDisable() {
		Settings.onSettingsChanged -= OnSettingsChanged;
	}	
}