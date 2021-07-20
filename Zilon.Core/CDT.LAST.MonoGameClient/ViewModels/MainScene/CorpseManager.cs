using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CDT.LAST.MonoGameClient.ViewModels.MainScene
{
    internal sealed class CorpseManager
    {
        private readonly IList<CorpseViewModel> _items;

        public CorpseManager()
        {
            _items = new List<CorpseViewModel>();
        }

        public void Add(CorpseViewModel corpseViewModel)
        {
            _items.Add(corpseViewModel);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var item in _items.ToArray())
            {
                item.Draw(spriteBatch);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var item in _items.ToArray())
            {
                item.Update(gameTime);

                if (item.IsComplete)
                {
                    _items.Remove(item);
                }
            }
        }
    }
}