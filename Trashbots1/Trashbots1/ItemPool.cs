using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Trashbots1
{
    public static class ItemPool
    {
        public static ContentManager Content;
        public static SpriteBatch spriteBatch;

        enum ItemTypes
        {
            Red, Green, Yellow, Blue
        }

        class Item
        {
            Texture2D texture;
            Vector2 position;
            float scale;
            public Item(ItemTypes itemType, Vector2 position, float scale)
            {
                switch (itemType)
                {
                    case ItemTypes.Yellow:
                        texture = Content.Load<Texture2D>("banana");
                        break;
                }
                this.position = position;
                this.scale = scale;
            }
            public void Update()
            {
            }
            public void Draw()
            {
                spriteBatch.Draw(texture, position, null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }
        }

        static List<Item> items = new List<Item>();

        public static void Start()
        {
            //TODO: Random for type, size, position
            items.Add(new Item(ItemTypes.Yellow, new Vector2(300),0.20f));
        }

        public static void Stop()
        {
            items.Clear();
        }

        public static void Update()
        {
            foreach (Item item in items)
            {
                item.Update();
            }
        }

        public static void Draw()
        {
            foreach (Item item in items)
            {
                item.Draw();
            }
        }





    }
}
