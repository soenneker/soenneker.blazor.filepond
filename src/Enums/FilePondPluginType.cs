using Ardalis.SmartEnum;

namespace Soenneker.Blazor.FilePond.Enums;

/// <summary>
/// This enum contains the names for the different file origins.
/// </summary>
public sealed class FilePondPluginType : SmartEnum<FilePondPluginType>
{
    /// <summary>
    /// Represents the FileEncode plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-encode/
    /// </summary>
    public static readonly FilePondPluginType FileEncode = new(nameof(FileEncode), 0);

    /// <summary>
    /// Represents the FileMetadata plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-metadata/
    /// </summary>
    public static readonly FilePondPluginType FileMetadata = new(nameof(FileMetadata), 1);

    /// <summary>
    /// Represents the FilePoster plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-poster/
    /// </summary>
    public static readonly FilePondPluginType FilePoster = new(nameof(FilePoster), 2);

    /// <summary>
    /// Represents the FileRename plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-rename/
    /// </summary>
    public static readonly FilePondPluginType FileRename = new(nameof(FileRename), 3);

    /// <summary>
    /// Represents the FileValidateSize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-size-validation/
    /// </summary>
    public static readonly FilePondPluginType FileValidateSize = new(nameof(FileValidateSize), 4);

    /// <summary>
    /// Represents the FileValidateType plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/file-type-validation/
    /// </summary>
    public static readonly FilePondPluginType FileValidateType = new(nameof(FileValidateType), 5);

    /// <summary>
    /// Represents the ImageCrop plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-crop/
    /// </summary>
    public static readonly FilePondPluginType ImageCrop = new(nameof(ImageCrop), 6);

    /// <summary>
    /// Represents the ImageEdit plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-edit/
    /// </summary>
    public static readonly FilePondPluginType ImageEdit = new(nameof(ImageEdit), 7);

    /// <summary>
    /// Represents the ImageExifOrientation plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-exif-orientation/
    /// </summary>
    public static readonly FilePondPluginType ImageExifOrientation = new(nameof(ImageExifOrientation), 8);

    /// <summary>
    /// Represents the ImageFilter plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-filter/
    /// </summary>
    public static readonly FilePondPluginType ImageFilter = new(nameof(ImageFilter), 9);

    /// <summary>
    /// Represents the ImagePreview plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-preview/
    /// </summary>
    public static readonly FilePondPluginType ImagePreview = new(nameof(ImagePreview), 10);

    /// <summary>
    /// Represents the ImageResize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-resize/
    /// </summary>
    public static readonly FilePondPluginType ImageResize = new(nameof(ImageResize), 11);

    /// <summary>
    /// Represents the ImageTransform plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-transform/
    /// </summary>
    public static readonly FilePondPluginType ImageTransform = new(nameof(ImageTransform), 12);

    /// <summary>
    /// Represents the ImageValidateSize plugin for FilePond.
    /// For more information, see: https://pqina.nl/filepond/docs/api/plugins/image-validate-size/
    /// </summary>
    public static readonly FilePondPluginType ImageValidateSize = new(nameof(ImageValidateSize), 13);

    private FilePondPluginType(string name, int value) : base(name, value)
    {
    }
}