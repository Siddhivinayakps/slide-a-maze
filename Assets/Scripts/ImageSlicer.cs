using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ImageSlicer
{
    public static Texture2D[,] GetSlices(Texture2D image, int blocksPerLine)
    {
        int imageSize = Mathf.Min(image.width, image.height);
        int blockSize = imageSize / blocksPerLine;

        Texture2D[,] blocks = new Texture2D[blocksPerLine, blocksPerLine];


        for (int x = 0; x < blocksPerLine; x++)
        {
            for (int y = 0; y < blocksPerLine; y++)
            {
                Texture2D block = new Texture2D(blockSize, blockSize);
                block.wrapMode = TextureWrapMode.Clamp;
                block.SetPixels(image.GetPixels(x * blockSize, y * blockSize, blockSize, blockSize));
                block.Apply();
                blocks[x, (blocksPerLine - 1) - y] = block;
            }
        }
        return blocks;
    }

    public static Sprite[,] GetSpriteSlices(Sprite SourceSprite, int blocksPerLine)
    {
        Sprite[,] blocks = new Sprite[blocksPerLine, blocksPerLine];
        int PixelsToUnits = SourceSprite.texture.height / blocksPerLine;
        int ColumnUnits = SourceSprite.texture.width / blocksPerLine;
        for (int row = 0; row < blocksPerLine; row++)
        {
            for (int column = 0; column < blocksPerLine; column++)
            {
                //Rect theArea = new Rect(row * PixelsToUnits, column * ColumnUnits, PixelsToUnits, ColumnUnits);
                Rect theArea = new Rect(column*ColumnUnits,row*PixelsToUnits,ColumnUnits,PixelsToUnits);



                blocks[blocksPerLine - 1 -row ,column] = Sprite.Create(SourceSprite.texture, theArea, Vector2.zero);
            }
        }
        return blocks;
    }
}
