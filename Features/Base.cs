using BepInEx.Logging;
using Comfort.Common;
using EFT;
using UnityEngine;

namespace Deminvincibility.Features;

internal abstract class BaseComponent : MonoBehaviour
{
    protected Player player;

    protected BaseComponent()
    {
        if (Logger == null) Logger = BepInEx.Logging.Logger.CreateLogSource(GetType().Name);
    }

    protected static ManualLogSource Logger { get; set; }

    protected void Start()
    {
        player = Singleton<GameWorld>.Instance.MainPlayer;
    }
}