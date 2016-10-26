using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace 엉덩국
{   // 오브젝트들
    class GameObject
    {
        public Texture2D sprite;        // 그림 경로 지정
        public Vector2 position;        // 위치
        public bool alive;              // 화면에서 보여지는 중인지
        public int framecount;          // 그림에서 보여질 위치
        public int delay;
        public Vector2 center;          // 중간 위치 지정

        public Vector2 velocity;        // 투사체 이동

        public GameObject(Texture2D loadedTexture)
        {
            position = Vector2.Zero;
            sprite = loadedTexture;
        }
    }
}
