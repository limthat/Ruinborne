using UnityEngine;
using UnityEngine.UIElements;
using Ruinborne.Network;

namespace Ruinborne.UI
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private NetworkRunnerHandler networkHandler;

        private VisualElement _root;

        private void OnEnable()
        {
            if (uiDocument == null) return;
            _root = uiDocument.rootVisualElement;

            _root.Q<Button>("btn-host")?.RegisterCallback<ClickEvent>(_ => OnHostClicked());
            _root.Q<Button>("btn-client")?.RegisterCallback<ClickEvent>(_ => OnClientClicked());
        }

        private void OnHostClicked()
        {
            Debug.Log("[LobbyUI] 호스트로 시작");
            if (networkHandler != null)
                networkHandler.StartHost();
            gameObject.SetActive(false);
        }

        private void OnClientClicked()
        {
            Debug.Log("[LobbyUI] 클라이언트로 시작");
            if (networkHandler != null)
                networkHandler.StartClient();
            gameObject.SetActive(false);
        }
    }
}
