using Birds.src.factories;
using Birds.src.utility;
using Birds.src.visual;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Birds.src.menu.controls;

public class EntityButtonManager
{
  private readonly List<EntityButton> buttons = new List<EntityButton>();
  private readonly List<IComponent> components;

  public EntityButtonManager(List<IComponent> components)
  {
    this.components = components;
  }

  public void CreateButtonGrid<T>(
      IEnumerable<KeyValuePair<T, ISprite>> previews,
      Action<T, EntityButton> onButtonClick,
      float scale = 3f,
      int buttonsPerRow = 3,
      float startX = 50f,
      float startY = 20f,
      float spacing = 5f,
      Func<T, bool> filter = null)
  {
    ClearButtons();

    int buttonIndex = 0;
    foreach (var kvp in previews)
    {
      if (filter != null && !filter(kvp.Key))
        continue;

      var button = CreateButton(kvp.Value, scale, buttonIndex, buttonsPerRow, startX, startY, spacing);
      button.Click += (sender, e) => onButtonClick(kvp.Key, sender as EntityButton);

      buttons.Add(button);
      components.Add(button);
      buttonIndex++;
    }
  }

  private EntityButton CreateButton(ISprite previewSprite, float scale, int index, int buttonsPerRow, float startX, float startY, float spacing)
  {
    int row = index / buttonsPerRow;
    int col = index % buttonsPerRow;

    float buttonWidth = SpriteFactory.textures[(int)ID_SPRITE.BUTTON_ENTITY].Width * scale;
    float buttonHeight = SpriteFactory.textures[(int)ID_SPRITE.BUTTON_ENTITY].Height * scale;

    float xPos = Game1.ScreenWidth - startX - (buttonWidth + spacing) * (buttonsPerRow - col);
    float yPos = startY + row * (buttonHeight + spacing);

    return new EntityButton(
        previewSprite,
        SpriteFactory.GetSprite(ID_SPRITE.BUTTON_ENTITY, Vector2.Zero, scale),
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

