using Birds.src.factories;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Birds.src.utility.factories;
using Birds.src.player;

namespace Birds.src.menu.controls;

public class EntityButtonManager(
  List<IComponent> components,
  Input input)
{
  private readonly List<EntityButton> buttons = new List<EntityButton>();

  public void CreateButtonGrid<T>(
      IEnumerable<KeyValuePair<T, ISprite>> previews,
      Action<T, EntityButton> onButtonClick,
      Action<string, EntityButton> longPressAction,
      float scale = 3f,
      int buttonsPerRow = 3,
      float startX = 50f,
      float startY = 20f,
      float spacing = 5f,
      float sectionSpacing = 30f,
      Func<T, bool> filter = null)
  {
    ClearButtons();

    var hullItems = new List<KeyValuePair<T, ISprite>>();
    var blueprintItems = new List<KeyValuePair<T, ISprite>>();

    foreach (var kvp in previews)
    {
      if (filter != null && !filter(kvp.Key)) continue;

      string name = kvp.Key.ToString();
      if (Enum.TryParse(typeof(ID_ENTITY), name, out _) && WorldEntityLoader.Hulls.Any(h => h.ToString() == name))
      {
        hullItems.Add(kvp);
      }
      else
      {
        blueprintItems.Add(kvp);
      }
    }

    int currentTotalIndex = 0;
    float currentYOffset = startY;

    if (hullItems.Count > 0)
    {
      foreach (var kvp in hullItems)
      {
        var button = CreateButton(kvp.Value, scale, currentTotalIndex, buttonsPerRow, startX, currentYOffset, spacing);
        SetupButton(button, kvp, onButtonClick, longPressAction);
        currentTotalIndex++;
      }

      int rowsUsed = (int)Math.Ceiling((double)hullItems.Count / buttonsPerRow);
      float buttonHeight = SpriteFactory.textures[(int)ID_SPRITE.ENTITY_BUTTON].Height * scale;
      currentYOffset += (rowsUsed * (buttonHeight + spacing)) + sectionSpacing;
    }

    int blueprintIndex = 0;
    foreach (var kvp in blueprintItems)
    {
      var button = CreateButton(kvp.Value, scale, blueprintIndex, buttonsPerRow, startX, currentYOffset, spacing);
      SetupButton(button, kvp, onButtonClick, longPressAction);
      blueprintIndex++;
    }
  }

  private void SetupButton<T>(EntityButton button, KeyValuePair<T, ISprite> kvp, Action<T, EntityButton> onButtonClick, Action<string, EntityButton> longPressAction)
  {
    string blueprintName = kvp.Key.ToString();
    button.Click += (sender, e) => onButtonClick(kvp.Key, sender as EntityButton);
    button.LongPress += (s, e) => longPressAction(blueprintName, button);
    buttons.Add(button);
    components.Add(button);
  }

  private EntityButton CreateButton(ISprite previewSprite, float scale, int index, int buttonsPerRow, float startX, float startY, float spacing)
  {
    int row = index / buttonsPerRow;
    int col = index % buttonsPerRow;

    float buttonWidth = SpriteFactory.textures[(int)ID_SPRITE.ENTITY_BUTTON].Width * scale;
    float buttonHeight = SpriteFactory.textures[(int)ID_SPRITE.ENTITY_BUTTON].Height * scale;

    float xPos = Game1.ScreenWidth - startX - (buttonWidth + spacing) * (buttonsPerRow - col);
    float yPos = startY + row * (buttonHeight + spacing);

    return new EntityButton(
        previewSprite,
        SpriteFactory.GetSprite(ID_SPRITE.ENTITY_BUTTON, Vector2.Zero, scale),
        input,
        autoFit: true)
    {
      Scale = scale,
      Position = new Vector2(xPos, yPos)
    };
  }

  public void ClearButtons()
  {
    foreach (var button in buttons)
    {
      components.Remove(button);
    }
    buttons.Clear();
  }

  public void SetFirstButtonSelected()
  {
    if (buttons.Count > 0)
    {
      buttons[0].IsClicked = true;
    }
  }

  public EntityButton GetFirstButton()
  {
    return buttons.FirstOrDefault();
  }
}