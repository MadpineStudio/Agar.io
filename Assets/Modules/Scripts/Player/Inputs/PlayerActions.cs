//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.11.2
//     from Assets/Modules/Scripts/Player/Inputs/PlayerActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerActions"",
    ""maps"": [
        {
            ""name"": ""PlayerMap"",
            ""id"": ""08f1c131-c7b1-48ad-9612-fb60c0dc424b"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""80dd1fba-d4aa-42fc-a883-e9a3375d97f5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Atacar"",
                    ""type"": ""Button"",
                    ""id"": ""e641f0c9-d5fe-46da-8f90-5ba7df32946f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Move"",
                    ""id"": ""ebeda63c-9012-40db-9f0b-e892162a04e0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""5a3ab7c3-510a-4afd-992b-cf0109bc215d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""6aba2089-670c-42f0-9bb2-a6e012b33317"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""57047f35-ce55-43c6-a460-d3d881719d9e"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""a16dc254-0cb0-4779-bd28-7618b58b9600"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e3ae921a-89e8-41e2-80f6-bd439ca95bdc"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Atacar"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerMap
        m_PlayerMap = asset.FindActionMap("PlayerMap", throwIfNotFound: true);
        m_PlayerMap_Move = m_PlayerMap.FindAction("Move", throwIfNotFound: true);
        m_PlayerMap_Atacar = m_PlayerMap.FindAction("Atacar", throwIfNotFound: true);
    }

    ~@PlayerActions()
    {
        UnityEngine.Debug.Assert(!m_PlayerMap.enabled, "This will cause a leak and performance issues, PlayerActions.PlayerMap.Disable() has not been called.");
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerMap
    private readonly InputActionMap m_PlayerMap;
    private List<IPlayerMapActions> m_PlayerMapActionsCallbackInterfaces = new List<IPlayerMapActions>();
    private readonly InputAction m_PlayerMap_Move;
    private readonly InputAction m_PlayerMap_Atacar;
    public struct PlayerMapActions
    {
        private @PlayerActions m_Wrapper;
        public PlayerMapActions(@PlayerActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_PlayerMap_Move;
        public InputAction @Atacar => m_Wrapper.m_PlayerMap_Atacar;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMapActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerMapActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Atacar.started += instance.OnAtacar;
            @Atacar.performed += instance.OnAtacar;
            @Atacar.canceled += instance.OnAtacar;
        }

        private void UnregisterCallbacks(IPlayerMapActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Atacar.started -= instance.OnAtacar;
            @Atacar.performed -= instance.OnAtacar;
            @Atacar.canceled -= instance.OnAtacar;
        }

        public void RemoveCallbacks(IPlayerMapActions instance)
        {
            if (m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerMapActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerMapActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerMapActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerMapActions @PlayerMap => new PlayerMapActions(this);
    public interface IPlayerMapActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnAtacar(InputAction.CallbackContext context);
    }
}
