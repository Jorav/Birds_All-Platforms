using Birds.src.containers.entity;
using Birds.src.events;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Birds.src.modules.composite;

public class SeparationDetectionModule : ModuleBase
{
  public List<IModuleContainer> SeparatedGroups { get; private set; } = new List<IModuleContainer>();

  protected override void Update(GameTime gameTime)
  {
  }

  public void CheckForSeparation()
  {
    var linkModule = container.GetModule<LinkManagementModule>();
    if (linkModule == null) return;

    var disconnectedGroups = linkModule.GetDisconnectedGroups();

    if (disconnectedGroups.Count > 1)
    {
      var primaryGroup = disconnectedGroups[0];

      for (int i = 1; i < disconnectedGroups.Count; i++)
      {
        var separatedGroup = CreateSeparatedComposite(disconnectedGroups[i]);
        SeparatedGroups.Add(separatedGroup);

        foreach (var entity in disconnectedGroups[i])
        {
          linkModule.RemoveEntity(entity);
        }
      }
    }
  }

  private IModuleContainer CreateSeparatedComposite(HashSet<IEntity> entities)
  {
    var separated = container.Clone() as IModuleContainer;
    separated.Entities.Clear();

    var separatedLinkModule = separated.GetModule<LinkManagementModule>();
    foreach (var entity in entities)
    {
      var clonedEntity = (IEntity)entity.Clone();
      separatedLinkModule?.AddEntity(clonedEntity);
    }

    return separated;
  }

  public override object Clone()
  {
    var cloned = new SeparationDetectionModule();
    cloned.SeparatedGroups = new List<IModuleContainer>();
    return cloned;
  }
}