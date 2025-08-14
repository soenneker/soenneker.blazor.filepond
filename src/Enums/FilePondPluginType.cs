using Intellenum;
// ReSharper disable InconsistentNaming

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// This enum contains the names for the different file origins.
/// </summary>
[Intellenum<string>]
public sealed partial class FilePondPluginType
{
    /// <summary>
    /// Represents the FileEncode plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-encode/
    /// </summary>
    public static readonly FilePondPluginType FileEncode = new(nameof(FileEncode));

    /// <summary>
    /// Represents the FileMetadata plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-metadata/
    /// </summary>
    public static readonly FilePondPluginType FileMetadata = new(nameof(FileMetadata));

    /// <summary>
    /// Represents the FilePoster plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-poster/
    /// </summary>
    public static readonly FilePondPluginType FilePoster = new(nameof(FilePoster));

    /// <summary>
    /// Represents the FileRename plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-rename/
    /// </summary>
    public static readonly FilePondPluginType FileRename = new(nameof(FileRename));

    /// <summary>
    /// Represents the FileValidateSize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-size-validation/
    /// </summary>
    public static readonly FilePondPluginType FileValidateSize = new(nameof(FileValidateSize));

    /// <summary>
    /// Represents the FileValidateType plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-type-validation/
    /// </summary>
    public static readonly FilePondPluginType FileValidateType = new(nameof(FileValidateType));

    /// <summary>
    /// Represents the ImageCrop plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-crop/
    /// </summary>
    public static readonly FilePondPluginType ImageCrop = new(nameof(ImageCrop));

    /// <summary>
    /// Represents the ImageEdit plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-edit/
    /// </summary>
    public static readonly FilePondPluginType ImageEdit = new(nameof(ImageEdit));

    /// <summary>
    /// Represents the ImageExifOrientation plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-exif-orientation/
    /// </summary>
    public static readonly FilePondPluginType ImageExifOrientation = new(nameof(ImageExifOrientation));

    /// <summary>
    /// Represents the ImageFilter plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-filter/
    /// </summary>
    public static readonly FilePondPluginType ImageFilter = new(nameof(ImageFilter));

    /// <summary>
    /// Represents the ImagePreview plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-preview/
    /// </summary>
    public static readonly FilePondPluginType ImagePreview = new(nameof(ImagePreview));

    /// <summary>
    /// Represents the ImageResize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-resize/
    /// </summary>
    public static readonly FilePondPluginType ImageResize = new(nameof(ImageResize));

    /// <summary>
    /// Represents the ImageTransform plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-transform/
    /// </summary>
    public static readonly FilePondPluginType ImageTransform = new(nameof(ImageTransform));

    /// <summary>
    /// Represents the ImageValidateSize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-validate-size/
    /// </summary>
    public static readonly FilePondPluginType ImageValidateSize = new(nameof(ImageValidateSize));

    public static readonly FilePondPluginType MediaPreview = new(nameof(MediaPreview));

    public static readonly FilePondPluginType ImageOverlay = new(nameof(ImageOverlay));

    public static readonly FilePondPluginType PdfPreview = new(nameof(PdfPreview));
}