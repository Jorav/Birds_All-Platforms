using Birds.src.events;
using Birds.src.player;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Birds.src.session.world;

public class WorldRenderer(
  World world,
  Camera camera)
{
  public void Draw(SpriteBatch spriteBatch)
  {
    spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(0.2f),
        sortMode: SpriteSortMode.Deferred,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.AnisotropicClamp);
    foreach (var b in world.Backgrounds) b.Draw(spriteBatch);
    spriteBatch.End();

    spriteBatch.Begin(transformMatrix: camera.Transform,
        sortMode: SpriteSortMode.Deferred,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.AnisotropicClamp);
    foreach (var c in world.Controllers) c.Draw(spriteBatch);
    spriteBatch.End();

    spriteBatch.Begin(transformMatrix: camera.GetParallaxTransform(1.5f),
        sortMode: SpriteSortMode.Deferred,
        blendState: BlendState.NonPremultiplied,
        samplerState: SamplerState.AnisotropicClamp);
    foreach (var f in world.Foregrounds) f.Draw(spriteBatch);
    spriteBatch.End();
  }
}
