using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class AtlasPacker : MonoBehaviour
{
    protected Dictionary<Texture2D, List<Image>> mImageMaps = new Dictionary<Texture2D, List<Image>>();
    List<Texture2D> textures = new List<Texture2D>();
    private MaxRectsBinPack packer;
    bool isSupportGpuCopy = true;
    Texture2D atlasTexture;
    int pixelsPerUnit = 100;

    private void Start()
    {
        if (SystemInfo.copyTextureSupport == CopyTextureSupport.None)
        {
            isSupportGpuCopy = false;
        }

        mImageMaps.Clear();
        textures.Clear();
        CollectSceneImage(GameObject.Find("Canvas"));
        CreateGameAtlasFiles();
    }

    void CreateGameAtlasFiles()
    {
        Stopwatch watch = Stopwatch.StartNew();
        List<MyRect> rectangles = new List<MyRect>();

        for (int i = 0; i < textures.Count; i++)
        {
            var texture = textures[i];
            rectangles.Add(new MyRect(0, 0, texture.width, texture.height));
        }

        int sizeW = 256, sizeH = 256;
        CreateBestRects(rectangles, ref sizeW, ref sizeH);

        if (atlasTexture == null)
            atlasTexture = new Texture2D(sizeW, sizeH, TextureFormat.RGB24, false);

        for (int i = 0; i < rectangles.Count; i++)
        {
            MyRect rect = rectangles[i];
            Texture2D texture = textures[i];
            if (isSupportGpuCopy)
                Graphics.CopyTexture(texture, 0, 0, 0, 0, rect.width, rect.height, atlasTexture, 0, 0, rect.x, rect.y);
            else
                atlasTexture.SetPixels32(rect.x, rect.y, rect.width, rect.height, texture.GetPixels32());
        }

        atlasTexture.Apply();

        for (int i = 0; i < rectangles.Count; i++)
        {
            MyRect rect = rectangles[i];
            Sprite sprite = Sprite.Create(atlasTexture, rect.ToRect(), Vector2.zero, pixelsPerUnit, 0,
                SpriteMeshType.FullRect);

            List<Image> list;
            if (mImageMaps.TryGetValue(textures[i], out list))
            {
                foreach (var image in list)
                {
                    image.sprite = sprite;
                }
            }
        }

        Debug.LogError(string.Format("耗时:{0}ms", watch.ElapsedMilliseconds));
    }

    public void CreateBestRects(List<MyRect> rectangles, ref int sizeW, ref int sizeH)
    {
        packer = new MaxRectsBinPack(sizeW, sizeH, false);

        int x, y;
        MyRect rectangle;
        for (int i = 0; i < rectangles.Count; i++)
        {
            rectangle = rectangles[i];
            if (!packer.TryAreaFit(rectangle.width, rectangle.height, out x, out y))
            {
                if (sizeW == sizeH)
                    sizeW *= 2;
                else
                    sizeH *= 2;
                CreateBestRects(rectangles, ref sizeW, ref sizeH);
                break;
            }

            rectangle.x = x;
            rectangle.y = y;
        }
    }

    void CollectSceneImage(GameObject obj)
    {
        if (obj == null)
            return;
        Image[] imags = obj.GetComponentsInChildren<Image>();
        foreach (var a in imags)
        {
            if (a.sprite != null && a.sprite.texture != null)
            {
                Texture2D texture = a.sprite.texture;

                if (texture.isReadable)
                {
                    List<Image> list;
                    if (!mImageMaps.TryGetValue(texture, out list))
                    {
                        list = mImageMaps[texture] = new List<Image>();
                    }

                    list.Add(a);
                    if (textures.Contains(texture))
                        continue;
                    textures.Add(texture);
                }
                else
                    Debug.LogError(texture + ":" + texture.isReadable);
            }
        }
    }

    public class MyRect
    {
        public int x;
        public int y;
        public int width;
        public int height;

        public MyRect(int x, int y, int w, int h)
        {
            this.x = x;
            this.y = y;
            this.width = w;
            this.height = h;
        }

        public Rect ToRect()
        {
            return new Rect(x, y, width, height);
        }

        public override string ToString()
        {
            return string.Format("x:{0} y:{1} widht:{2} height:{3}", x, y , width , height);
        }
    }
}