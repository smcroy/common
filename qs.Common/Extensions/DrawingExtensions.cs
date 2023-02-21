namespace qs.Extensions.DrawingExtensions
{
    using qs.Extensions.DoubleExtensions;
    using qs.Extensions.Int32Extensions;
    using qs.Extensions.ReflectionExtensions;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Reflection;

    public static class DrawingExtensions
    {
        #region Enumerations

        public enum BorderWidth
        {
            Tiny = 3,
            Small = 7,
            Moderate = 14,
            Large = 21
        }

        public enum ShadowDistance
        {
            Tiny = 5,
            Normal = 7,
            Small = 10,
            Moderate = 15,
            Large = 20
        }

        public enum ShadowOpacity
        {
            Most = 100,
            Moderate = 130,
            Least = 200
        }

        public enum ShadowSoftness
        {
            Hard = 2,
            Moderate = 4,
            Soft = 6,
            Very = 8,
            Extreme = 10
        }

        public enum ShadowStyle
        {
            None,
            All,
            TopRight,
            BottomRight,
            BottomLeft,
            TopLeft
        }

        #endregion Enumerations

        #region Methods

        public static Image AddBorder(this Image b, Color borderColor, BorderWidth borderWidth)
        {
            return b.AddBorder(borderColor, (int)borderWidth);
        }

        public static Image AddBorder(this Image b, Color borderColor, int borderWidth)
        {
            Image sourceImage = b as Image;
            if (sourceImage == null)
                return null;

            int sourceImageWidth = sourceImage.Width;
            int sourceImageHeight = sourceImage.Height;

            Image resizedImage = sourceImage.Resize(new Size(sourceImageWidth - (borderWidth * 2), sourceImageHeight - (borderWidth * 2)));

            Image resultImage = null;

            using (Image borderImage = new Bitmap(sourceImageWidth, sourceImageHeight))
            {
                using (Graphics g = Graphics.FromImage(borderImage))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRectangle(new SolidBrush(Color.FromArgb(255, borderColor)), 0, 0, sourceImageWidth, sourceImageHeight);
                }

                using (Image targetImage = new Bitmap(sourceImageWidth, sourceImageHeight))
                {
                    using (Graphics g = Graphics.FromImage(targetImage))
                    {
                        g.Clear(borderColor);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawImage(borderImage, new Rectangle(0, 0, sourceImageWidth, sourceImageHeight), 0, 0, sourceImageWidth, sourceImageHeight, GraphicsUnit.Pixel);
                        g.DrawImage(resizedImage, new Rectangle(borderWidth, borderWidth, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                    }

                    resultImage = new Bitmap(targetImage);
                }
            }

            return resultImage;
        }

        public static Image AddShadow(this Image b, Color shadowColor, Color backgroundColor, ShadowStyle shadowStyle, ShadowDistance shadowDistance, ShadowSoftness shadowSoftness, ShadowOpacity shadowOpacity)
        {
            return b.AddShadow(shadowColor, backgroundColor, shadowStyle, (int)shadowDistance, (int)shadowSoftness, (int)shadowOpacity);
        }

        public static Image AddShadow(this Image b, Color shadowColor, Color backgroundColor, ShadowStyle shadowStyle, int shadowDistance, int shadowSoftness, int shadowOpacity)
        {
            Image sourceImage = b as Image;
            if (sourceImage == null)
                return null;

            if (shadowStyle == ShadowStyle.None)
                return sourceImage;

            int sourceImageWidth = sourceImage.Width;
            int sourceImageHeight = sourceImage.Height;

            Image resizedImage = null;

            switch (shadowStyle)
            {
                case ShadowStyle.All:
                    resizedImage = sourceImage.Resize(new Size(sourceImageWidth - (shadowDistance * 2), sourceImageHeight - (shadowDistance * 2)));
                    break;

                case ShadowStyle.TopRight:
                case ShadowStyle.BottomRight:
                case ShadowStyle.BottomLeft:
                case ShadowStyle.TopLeft:
                default:
                    resizedImage = sourceImage.Resize(new Size(sourceImageWidth - shadowDistance, sourceImageHeight - shadowDistance));
                    break;
            }

            Image resultImage = null;

            int w = sourceImage.Width / shadowSoftness;
            int h = sourceImage.Height / shadowSoftness;

            using (Image shadowImage = new Bitmap(w, h))
            {
                using (Graphics g = Graphics.FromImage(shadowImage))
                {
                    g.Clear(Color.Transparent);
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.FillRectangle(new SolidBrush(Color.FromArgb(shadowOpacity, shadowColor)), 1, 1, w, h);
                }

                using (Image targetImage = new Bitmap(sourceImageWidth, sourceImageHeight))
                {
                    using (Graphics g = Graphics.FromImage(targetImage))
                    {
                        g.Clear(backgroundColor);
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawImage(shadowImage, new Rectangle(0, 0, sourceImageWidth, sourceImageHeight), 0, 0, shadowImage.Width, shadowImage.Height, GraphicsUnit.Pixel);
                    }

                    switch (shadowStyle)
                    {
                        case ShadowStyle.BottomLeft:
                            using (Graphics g = Graphics.FromImage(targetImage))
                            {
                                g.DrawImage(resizedImage, new Rectangle(shadowDistance, 0, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                            }
                            break;

                        case ShadowStyle.TopLeft:
                            targetImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            using (Graphics g = Graphics.FromImage(targetImage))
                            {
                                g.DrawImage(resizedImage, new Rectangle(shadowDistance, shadowDistance, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                            }
                            break;

                        case ShadowStyle.TopRight:
                            targetImage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                            using (Graphics g = Graphics.FromImage(targetImage))
                            {
                                g.DrawImage(resizedImage, new Rectangle(0, shadowDistance, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                            }
                            break;

                        case ShadowStyle.All:
                            using (Graphics g = Graphics.FromImage(targetImage))
                            {
                                g.DrawImage(resizedImage, new Rectangle(shadowDistance + 1, shadowDistance + 1, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                            }
                            break;

                        case ShadowStyle.BottomRight:
                        default:
                            using (Graphics g = Graphics.FromImage(targetImage))
                            {
                                g.DrawImage(resizedImage, new Rectangle(0, 0, resizedImage.Width, resizedImage.Height), 0, 0, resizedImage.Width, resizedImage.Height, GraphicsUnit.Pixel);
                            }
                            break;
                    }

                    resultImage = new Bitmap(targetImage);
                }
            }

            return resultImage;
        }

        public static int CompareTo(this Size a, Size b)
        {
            if (a.Height == b.Height && a.Width == b.Width)
                return 0;
            if (a.Height > b.Height || a.Width > b.Width)
                return 1;
            return -1;
        }

        public static Image Crop(this Image image, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(image);
            Bitmap bmpCrop = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
            return (Image)(bmpCrop);
        }

        public static Icon Overlay(this Icon b, Icon overlayIcon, Point overlayAtPoint, Size newSize)
        {
            Icon o = overlayIcon as Icon;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            return overlay(b, new Point(0, 0), b.Size, o, overlayAtPoint, o.Size, newSize);
        }

        public static Image Overlay(this Image b, Image overlayImage, Point overlayAtPoint, Size newSize)
        {
            Image o = overlayImage as Image;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            return overlay(b, new Point(0, 0), b.Size, o, overlayAtPoint, o.Size, newSize);
        }

        public static Icon Overlay(this Icon b, Icon overlayIcon, Point overlayAtPoint, Size overlaySize, Size newSize)
        {
            Icon o = overlayIcon as Icon;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            return overlay(b, new Point(0, 0), b.Size, o, overlayAtPoint, overlaySize, newSize);
        }

        public static Image Overlay(this Image b, Image overlayImage, Point overlayAtPoint, Size overlaySize, Size newSize)
        {
            Image o = overlayImage as Image;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            return overlay(b, new Point(0, 0), b.Size, o, overlayAtPoint, overlaySize, newSize);
        }

        public static Image Resize(this Image image, Size newSize)
        {
            return new Bitmap(image, newSize);
        }

        public static Icon Resize(this Icon icon, Size newSize)
        {
            using (Bitmap bitmap = new Bitmap(icon.ToBitmap(), newSize))
            {
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }

        public static Image ResizeAndMaintainAspect(this Image image, Size newSize)
        {
            Size a = image.Size;
            Size b = newSize;
            if (a.Width > a.Height && b.Width < b.Height)
                b = new Size(newSize.Height, newSize.Width);

            double r = a.Height.ToDouble() / a.Width.ToDouble();
            b.Height = (b.Width * r).ToInt32();
            if (b.Height > newSize.Height)
            {
                b.Height = newSize.Height;
                b.Width = (b.Height / r).ToInt32();
            }
            return new Bitmap(image, b);
        }

        public static byte[] ToArray(this Image image)
        {
            return image.ToArray(ImageFormat.Jpeg);
        }

        public static byte[] ToArray(this Image image, ImageFormat format)
        {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, format);
            return stream.ToArray();
        }

        public static Icon ToBleach(this Icon icon)
        {
            return bleach(icon, bleachConstants.bleachNormal, icon.Size);
        }

        public static Image ToBleach(this Image image)
        {
            return bleach(image, bleachConstants.bleachNormal, image.Size);
        }

        public static Icon ToBleach(this Icon icon, Size newSize)
        {
            return bleach(icon, bleachConstants.bleachNormal, newSize);
        }

        public static Image ToBleach(this Image image, Size newSize)
        {
            return bleach(image, bleachConstants.bleachNormal, newSize);
        }

        public static Icon ToBleach(this Icon icon, float amount)
        {
            return bleach(icon, amount, icon.Size);
        }

        public static Image ToBleach(this Image image, float amount)
        {
            return bleach(image, amount, image.Size);
        }

        public static Icon ToBleach(this Icon icon, float amount, Size newSize)
        {
            return bleach(icon, amount, newSize);
        }

        public static Image ToBleach(this Image image, float amount, Size newSize)
        {
            return bleach(image, amount, newSize);
        }

        public static Icon ToDropShadow(this Icon icon)
        {
            return dropShadow(icon, dropShadowConstants.offsetNormal, dropShadowConstants.shadowNormal, icon.Size);
        }

        public static Image ToDropShadow(this Image image)
        {
            return dropShadow(image, dropShadowConstants.offsetNormal, dropShadowConstants.shadowNormal, image.Size);
        }

        public static Icon ToDropShadow(this Icon icon, Size newSize)
        {
            return dropShadow(icon, dropShadowConstants.offsetNormal, dropShadowConstants.shadowNormal, newSize);
        }

        public static Image ToDropShadow(this Image image, Size newSize)
        {
            return dropShadow(image, dropShadowConstants.offsetNormal, dropShadowConstants.shadowNormal, newSize);
        }

        public static Icon ToDropShadow(this Icon icon, byte offset)
        {
            return dropShadow(icon, offset, dropShadowConstants.shadowNormal, icon.Size);
        }

        public static Image ToDropShadow(this Image image, byte offset)
        {
            return dropShadow(image, offset, dropShadowConstants.shadowNormal, image.Size);
        }

        public static Icon ToDropShadow(this Icon icon, byte offset, Size newSize)
        {
            return dropShadow(icon, offset, dropShadowConstants.shadowNormal, newSize);
        }

        public static Image ToDropShadow(this Image image, byte offset, Size newSize)
        {
            return dropShadow(image, offset, dropShadowConstants.shadowNormal, newSize);
        }

        public static Icon ToDropShadow(this Icon icon, float shadow)
        {
            return dropShadow(icon, dropShadowConstants.offsetNormal, shadow, icon.Size);
        }

        public static Image ToDropShadow(this Image image, float shadow)
        {
            return dropShadow(image, dropShadowConstants.offsetNormal, shadow, image.Size);
        }

        public static Icon ToDropShadow(this Icon icon, float shadow, Size newSize)
        {
            return dropShadow(icon, dropShadowConstants.offsetNormal, shadow, newSize);
        }

        public static Image ToDropShadow(this Image image, float shadow, Size newSize)
        {
            return dropShadow(image, dropShadowConstants.offsetNormal, shadow, newSize);
        }

        public static Icon ToDropShadow(this Icon icon, byte offset, float shadow)
        {
            return dropShadow(icon, offset, shadow, icon.Size);
        }

        public static Image ToDropShadow(this Image image, byte offset, float shadow)
        {
            return dropShadow(image, offset, shadow, image.Size);
        }

        public static Icon ToDropShadow(this Icon icon, byte offset, float shadow, Size newSize)
        {
            return dropShadow(icon, offset, shadow, newSize);
        }

        public static Image ToDropShadow(this Image image, byte offset, float shadow, Size newSize)
        {
            return dropShadow(image, offset, shadow, newSize);
        }

        public static Icon ToGreyScale(this Icon icon)
        {
            return greyScale(icon, greyScaleConstants.scaleNormal, icon.Size);
        }

        public static Image ToGreyScale(this Image image)
        {
            return greyScale(image, greyScaleConstants.scaleNormal, image.Size);
        }

        public static Icon ToGreyScale(this Icon icon, Size newSize)
        {
            return greyScale(icon, greyScaleConstants.scaleNormal, newSize);
        }

        public static Image ToGreyScale(this Image image, Size newSize)
        {
            return greyScale(image, greyScaleConstants.scaleNormal, newSize);
        }

        public static Icon ToGreyScale(this Icon icon, float amount)
        {
            return greyScale(icon, amount, icon.Size);
        }

        public static Image ToGreyScale(this Image image, float amount)
        {
            return greyScale(image, amount, image.Size);
        }

        public static Icon ToGreyScale(this Icon icon, float amount, Size newSize)
        {
            return greyScale(icon, amount, newSize);
        }

        public static Image ToGreyScale(this Image image, float amount, Size newSize)
        {
            return greyScale(image, amount, newSize);
        }

        public static Icon ToIcon(this Image image)
        {
            using (Bitmap bitmap = new Bitmap(image))
            {
                return Icon.FromHandle(bitmap.GetHicon());
            }
        }

        /// <summary>
        /// Convert this instance of an System.Drawing.Icon to an instance of System.Drawing.Image.
        /// </summary>
        /// <param path="icon"></param>
        /// <returns></returns>
        public static Image ToImage(this Icon icon)
        {
            return icon.ToBitmap();
        }

        public static Icon ToTranslucent(this Icon icon)
        {
            return translucent(icon, translucentConstants.translucentNormal, icon.Size);
        }

        public static Image ToTranslucent(this Image image)
        {
            return translucent(image, translucentConstants.translucentNormal, image.Size);
        }

        public static Icon ToTranslucent(this Icon icon, Size newSize)
        {
            return translucent(icon, translucentConstants.translucentNormal, newSize);
        }

        public static Image ToTranslucent(this Image image, Size newSize)
        {
            return translucent(image, translucentConstants.translucentNormal, newSize);
        }

        public static Icon ToTranslucent(this Icon icon, float amount)
        {
            return translucent(icon, amount, icon.Size);
        }

        public static Image ToTranslucent(this Image image, float amount)
        {
            return translucent(image, amount, image.Size);
        }

        public static Icon ToTranslucent(this Icon icon, float amount, Size newSize)
        {
            return translucent(icon, amount, newSize);
        }

        public static Image ToTranslucent(this Image image, float amount, Size newSize)
        {
            return translucent(image, amount, newSize);
        }

        private static Icon bleach(Icon icon, float amount, Size destSize)
        {
            Icon o = icon as Icon;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = 1;
                            cm.Matrix11 = 1;
                            cm.Matrix22 = 1;
                            cm.Matrix33 = amount;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o.ToBitmap(), new Rectangle(0, 0, destSize.Width, destSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return Icon.FromHandle(bitmap.GetHicon());
                        }
                    }
                }
            }
        }

        private static Image bleach(Image image, float amount, Size destSize)
        {
            Image o = image as Image;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = 1;
                            cm.Matrix11 = 1;
                            cm.Matrix22 = 1;
                            cm.Matrix33 = amount;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o, new Rectangle(0, 0, destSize.Width, destSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return (Image)bitmap.Clone();
                        }
                    }
                }
            }
        }

        private static Icon dropShadow(Icon icon, byte offset, float shadow, Size destSize)
        {
            Icon o = icon as Icon;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.DrawImage(ToGreyScale(o, shadow, destSize).ToBitmap(), new Rectangle(offset, offset, destSize.Width - offset, destSize.Height - offset));
                        graphics.DrawImage(o.ToBitmap(), new Rectangle(0, 0, destSize.Width - offset, destSize.Height - offset), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel);

                        return Icon.FromHandle(bitmap.GetHicon());
                    }
                }
            }
        }

        private static Image dropShadow(Image image, byte offset, float shadow, Size destSize)
        {
            Image o = image as Image;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        graphics.DrawImage(ToGreyScale(image, shadow, destSize), new Rectangle(offset, offset, destSize.Width - offset, destSize.Height - offset));
                        graphics.DrawImage(image, new Rectangle(0, 0, destSize.Width - offset, destSize.Height - offset), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel);

                        return (Image)bitmap.Clone();
                    }
                }
            }
        }

        private static Icon greyScale(Icon icon, float amount, Size newSize)
        {
            Icon o = icon as Icon;
            if (o == null)
                return null;
            else
            {
                float a = amount;
                float b = 0.837375f * (1.0f - amount);
                float c = 0.162625f * (1.0f - amount);

                using (Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = cm.Matrix01 = cm.Matrix02 = a;
                            cm.Matrix10 = cm.Matrix11 = cm.Matrix12 = b;
                            cm.Matrix20 = cm.Matrix21 = cm.Matrix22 = c;
                            cm.Matrix33 = 1;
                            cm.Matrix44 = 1;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o.ToBitmap(), new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return Icon.FromHandle(bitmap.GetHicon());
                        }
                    }
                }
            }
        }

        private static Image greyScale(Image image, float amount, Size newSize)
        {
            Image o = image as Image;
            if (o == null)
                return null;
            else
            {
                float a = amount;
                float b = 0.837375f * (1.0f - amount);
                float c = 0.162625f * (1.0f - amount);

                using (Bitmap bitmap = new Bitmap(newSize.Width, newSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = cm.Matrix01 = cm.Matrix02 = a;
                            cm.Matrix10 = cm.Matrix11 = cm.Matrix12 = b;
                            cm.Matrix20 = cm.Matrix21 = cm.Matrix22 = c;
                            cm.Matrix33 = 1;
                            cm.Matrix44 = 1;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o, new Rectangle(0, 0, newSize.Width, newSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return (Image)bitmap.Clone();
                        }
                    }
                }
            }
        }

        private static Icon overlay(Icon baseIcon, Point baseIconAtPoint, Size baseIconSize, Icon overlayIcon, Point overlayIconAtPoint, Size overlayIconSize, Size destSize)
        {
            Icon b = baseIcon as Icon;
            if (b == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(0));

            Icon o = overlayIcon as Icon;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.DrawImage(Resize(b, destSize).ToBitmap(), new Rectangle(baseIconAtPoint, baseIconSize), 0, 0, baseIconSize.Width, baseIconSize.Height, GraphicsUnit.Pixel);
                    graphics.DrawImage(o.ToBitmap(), new Rectangle(overlayIconAtPoint, overlayIconSize));

                    return Icon.FromHandle(bitmap.GetHicon());
                }
            }
        }

        private static Image overlay(Image baseImage, Point baseImageAtPoint, Size baseImageSize, Image overlayImage, Point overlayImageAtPoint, Size overlayImageSize, Size destSize)
        {
            Image b = baseImage as Image;
            if (b == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(0));

            Image o = overlayImage as Image;
            if (o == null)
                throw new ArgumentNotOfTypeException(MethodBase.GetCurrentMethod().GetParameterInfo(1));

            using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.DrawImage(Resize(b, destSize), new Rectangle(baseImageAtPoint, baseImageSize), 0, 0, baseImageSize.Width, baseImageSize.Height, GraphicsUnit.Pixel);
                    graphics.DrawImage(o, new Rectangle(overlayImageAtPoint, overlayImageSize));

                    return (Image)bitmap.Clone();
                }
            }
        }

        private static Icon translucent(Icon icon, float amount, Size destSize)
        {
            Icon o = icon as Icon;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = 1;
                            cm.Matrix11 = 1;
                            cm.Matrix22 = 1;
                            cm.Matrix33 = amount;
                            cm.Matrix44 = 1;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o.ToBitmap(), new Rectangle(0, 0, destSize.Width, destSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return Icon.FromHandle(bitmap.GetHicon());
                        }
                    }
                }
            }
        }

        private static Image translucent(Image image, float amount, Size destSize)
        {
            Image o = image as Image;
            if (o == null)
                return null;
            else
            {
                using (Bitmap bitmap = new Bitmap(destSize.Width, destSize.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.SmoothingMode = SmoothingMode.AntiAlias;
                        using (ImageAttributes ia = new ImageAttributes())
                        {
                            ColorMatrix cm = new ColorMatrix();

                            cm.Matrix00 = 1;
                            cm.Matrix11 = 1;
                            cm.Matrix22 = 1;
                            cm.Matrix33 = amount;
                            cm.Matrix44 = 1;

                            ia.SetColorMatrix(cm);
                            graphics.DrawImage(o, new Rectangle(0, 0, destSize.Width, destSize.Height), 0, 0, o.Width, o.Height, GraphicsUnit.Pixel, ia);

                            return (Image)bitmap.Clone();
                        }
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        internal static class bleachConstants
        {
            #region Fields

            internal static float bleachNormal = 0.5f;

            #endregion Fields
        }

        internal static class dropShadowConstants
        {
            #region Fields

            internal static byte offsetNormal = 2;
            internal static float shadowNormal = 0.1f;

            #endregion Fields
        }

        internal static class greyScaleConstants
        {
            #region Fields

            internal static float scaleNormal = 0.3f;

            #endregion Fields
        }

        internal static class translucentConstants
        {
            #region Fields

            internal static float translucentNormal = 0.8f;

            #endregion Fields
        }

        #endregion Nested Types
    }
}