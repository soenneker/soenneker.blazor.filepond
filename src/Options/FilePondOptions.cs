using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Soenneker.Blazor.FilePond.Options;

/// <summary>
/// Represents the properties of the FilePond instance.
/// </summary>
public class FilePondOptions
{
    /// <summary>
    /// Gets or sets the input field name to use. Default value is 'filepond'.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "filepond";

    /// <summary>
    /// Gets or sets an additional CSS class to add to the root element.
    /// </summary>
    [JsonPropertyName("className")]
    public string? ClassName { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the output field is required.
    /// </summary>
    [JsonPropertyName("required")]
    public bool Required { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the output field is disabled.
    /// </summary>
    [JsonPropertyName("disabled")]
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// Gets or sets the value to set to the capture attribute.
    /// </summary>
    [JsonPropertyName("captureMethod")]
    public string? CaptureMethod { get; set; } = null;

    /// <summary>
    /// Gets or sets a list of file locations that should be loaded immediately.
    /// </summary>
    [JsonPropertyName("files")]
    public List<string> Files { get; set; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether drag n' drop is enabled.
    /// </summary>
    [JsonPropertyName("allowDrop")]
    public bool AllowDrop { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the file browser is enabled.
    /// </summary>
    [JsonPropertyName("allowBrowse")]
    public bool AllowBrowse { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether pasting of files is enabled.
    /// Pasting files is not supported on all browsers.
    /// </summary>
    [JsonPropertyName("allowPaste")]
    public bool AllowPaste { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether adding multiple files is enabled. Default is false (disabled).
    /// </summary>
    [JsonPropertyName("allowMultiple")]
    public bool AllowMultiple { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether drop can replace a file. Only works when allowMultiple is false.
    /// </summary>
    [JsonPropertyName("allowReplace")]
    public bool AllowReplace { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the revert processing button is enabled.
    /// </summary>
    [JsonPropertyName("allowRevert")]
    public bool AllowRevert { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the remove button is enabled.
    /// When set to false, the remove button is hidden and disabled.
    /// </summary>
    [JsonPropertyName("allowRemove")]
    public bool AllowRemove { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the process button is enabled.
    /// </summary>
    [JsonPropertyName("allowProcess")]
    public bool AllowProcess { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether users can reorder files with drag and drop interaction.
    /// Note that this only works in single column mode and on browsers that support pointer events.
    /// </summary>
    [JsonPropertyName("allowReorder")]
    public bool AllowReorder { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether FilePond should store files in hidden file input elements.
    /// Files can be posted along with normal form post. This only works if the browser supports the DataTransfer constructor.
    /// </summary>
    [JsonPropertyName("storeAsFile")]
    public bool StoreAsFile { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the file must be successfully reverted before continuing.
    /// </summary>
    [JsonPropertyName("forceRevert")]
    public bool ForceRevert { get; set; } = false;

    /// <summary>
    /// Gets or sets the maximum number of files that the pond can handle.
    /// </summary>
    [JsonPropertyName("maxFiles")]
    public int? MaxFiles { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of files that can be uploaded in parallel.
    /// </summary>
    [JsonPropertyName("maxParallelUploads")]
    public int MaxParallelUploads { get; set; } = 2;

    /// <summary>
    /// Gets or sets a value indicating whether to check validity for custom validity messages.
    /// FilePond will throw an error when a parent form is submitted and it contains invalid files.
    /// </summary>
    [JsonPropertyName("checkValidity")]
    public bool CheckValidity { get; set; } = false;

    /// <summary>
    /// Gets or sets the item insert location. Set to 'after' to add files to the end of the list (when dropped at the top of the list or added using browse or paste).
    /// Set to 'before' to add files at the start of the list. Set to a compare function to automatically sort items when added.
    /// </summary>
    [JsonPropertyName("itemInsertLocation")]
    public string ItemInsertLocation { get; set; } = "before";

    /// <summary>
    /// Gets or sets the interval to use before showing each item being added to the list.
    /// </summary>
    [JsonPropertyName("itemInsertInterval")]
    public int ItemInsertInterval { get; set; } = 75;

    /// <summary>
    /// Gets or sets the base to use for file size calculations. This is only used for displaying file sizes.
    /// </summary>
    [JsonPropertyName("fileSizeBase")]
    public int FileSizeBase { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the credits information. Shows Powered by PQINA in the footer.
    /// </summary>
    [JsonPropertyName("credits")]
    public Dictionary<string, string>? Credits { get; set; }

    #region Labels

    /// <summary>
    /// Gets or sets the decimal separator used to render numbers.
    /// By default, this is determined automatically.
    /// </summary>
    [JsonPropertyName("labelDecimalSeparator")]
    public string LabelDecimalSeparator { get; set; } = "undefined";

    /// <summary>
    /// Gets or sets the thousands separator used to render numbers.
    /// By default, this is determined automatically.
    /// </summary>
    [JsonPropertyName("labelThousandsSeparator")]
    public string LabelThousandsSeparator { get; set; } = "undefined";

    /// <summary>
    /// Gets or sets the default label shown to indicate this is a drop area.
    /// FilePond will automatically bind browse file events to the element with CSS class .filepond--label-action.
    /// </summary>
    [JsonPropertyName("labelIdle")]
    public string LabelIdle { get; set; } = "Drag & Drop your files or <span class=\"filepond--label-action\"> Browse </span>";

    /// <summary>
    /// Gets or sets the label shown when the field contains invalid files and is validated by the parent form.
    /// </summary>
    [JsonPropertyName("labelInvalidField")]
    public string LabelInvalidField { get; set; } = "Field contains invalid files";

    /// <summary>
    /// Gets or sets the label used while waiting for file size information.
    /// </summary>
    [JsonPropertyName("labelFileWaitingForSize")]
    public string LabelFileWaitingForSize { get; set; } = "Waiting for size";

    /// <summary>
    /// Gets or sets the label used when no file size information was received.
    /// </summary>
    [JsonPropertyName("labelFileSizeNotAvailable")]
    public string LabelFileSizeNotAvailable { get; set; } = "Size not available";

    /// <summary>
    /// Gets or sets the label used while loading a file.
    /// </summary>
    [JsonPropertyName("labelFileLoading")]
    public string LabelFileLoading { get; set; } = "Loading";

    /// <summary>
    /// Gets or sets the label used when file load failed.
    /// </summary>
    [JsonPropertyName("labelFileLoadError")]
    public string LabelFileLoadError { get; set; } = "Error during load";

    /// <summary>
    /// Gets or sets the label used when uploading a file.
    /// </summary>
    [JsonPropertyName("labelFileProcessing")]
    public string LabelFileProcessing { get; set; } = "Uploading";

    /// <summary>
    /// Gets or sets the label used when file upload has completed.
    /// </summary>
    [JsonPropertyName("labelFileProcessingComplete")]
    public string LabelFileProcessingComplete { get; set; } = "Upload complete";

    /// <summary>
    /// Gets or sets the label used when upload was cancelled.
    /// </summary>
    [JsonPropertyName("labelFileProcessingAborted")]
    public string LabelFileProcessingAborted { get; set; } = "Upload cancelled";

    /// <summary>
    /// Gets or sets the label used when something went wrong during file upload.
    /// </summary>
    [JsonPropertyName("labelFileProcessingError")]
    public string LabelFileProcessingError { get; set; } = "Error during upload";

    /// <summary>
    /// Gets or sets the label used when something went wrong during reverting the file upload.
    /// </summary>
    [JsonPropertyName("labelFileProcessingRevertError")]
    public string LabelFileProcessingRevertError { get; set; } = "Error during revert";

    /// <summary>
    /// Gets or sets the label used to indicate something went wrong when removing the file.
    /// </summary>
    [JsonPropertyName("labelFileRemoveError")]
    public string LabelFileRemoveError { get; set; } = "Error during remove";

    /// <summary>
    /// Gets or sets the label used to indicate to the user that an action can be cancelled.
    /// </summary>
    [JsonPropertyName("labelTapToCancel")]
    public string LabelTapToCancel { get; set; } = "tap to cancel";

    /// <summary>
    /// Gets or sets the label used to indicate to the user that an action can be retried.
    /// </summary>
    [JsonPropertyName("labelTapToRetry")]
    public string LabelTapToRetry { get; set; } = "tap to retry";

    /// <summary>
    /// Gets or sets the label used to indicate to the user that an action can be undone.
    /// </summary>
    [JsonPropertyName("labelTapToUndo")]
    public string LabelTapToUndo { get; set; } = "tap to undo";

    /// <summary>
    /// Gets or sets the label used for the remove button.
    /// </summary>
    [JsonPropertyName("labelButtonRemoveItem")]
    public string LabelButtonRemoveItem { get; set; } = "Remove";

    /// <summary>
    /// Gets or sets the label used for the abort load button.
    /// </summary>
    [JsonPropertyName("labelButtonAbortItemLoad")]
    public string LabelButtonAbortItemLoad { get; set; } = "Abort";

    /// <summary>
    /// Gets or sets the label used for the retry load button.
    /// </summary>
    [JsonPropertyName("labelButtonRetryItemLoad")]
    public string LabelButtonRetryItemLoad { get; set; } = "Retry";

    /// <summary>
    /// Gets or sets the label used for the abort upload button.
    /// </summary>
    [JsonPropertyName("labelButtonAbortItemProcessing")]
    public string LabelButtonAbortItemProcessing { get; set; } = "Cancel";

    /// <summary>
    /// Gets or sets the label used for the undo upload button.
    /// </summary>
    [JsonPropertyName("labelButtonUndoItemProcessing")]
    public string LabelButtonUndoItemProcessing { get; set; } = "Undo";

    /// <summary>
    /// Gets or sets the label used for the retry upload button.
    /// </summary>
    [JsonPropertyName("labelButtonRetryItemProcessing")]
    public string LabelButtonRetryItemProcessing { get; set; } = "Retry";

    /// <summary>
    /// Gets or sets the label used for the upload button.
    /// </summary>
    [JsonPropertyName("labelButtonProcessItem")]
    public string LabelButtonProcessItem { get; set; } = "Upload";

    #endregion Labels

    /// <summary>
    /// Gets or sets a value indicating whether FilePond will catch all files dropped on the webpage.
    /// </summary>
    [JsonPropertyName("dropOnPage")]
    public bool DropOnPage { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether dropping on the FilePond element itself is required to catch the file.
    /// </summary>
    [JsonPropertyName("dropOnElement")]
    public bool DropOnElement { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether drop validation is enabled.
    /// When enabled, files are validated before they are dropped. A file is not added when it's invalid.
    /// </summary>
    [JsonPropertyName("dropValidation")]
    public bool DropValidation { get; set; } = false;

    /// <summary>
    /// Gets or sets the ignored file names when handling dropped directories.
    /// Dropping directories is not supported on all browsers.
    /// </summary>
    [JsonPropertyName("ignoredFiles")]
    public List<string> IgnoredFiles { get; set; } =
    [
        ".ds_store",
        "thumbs.db",
        "desktop.ini"
    ];

    /// <summary>
    /// Gets or sets a server configuration object describing how FilePond should interact with the server.
    /// </summary>
    [JsonPropertyName("server")]
    public FilePondServerOptions? Server { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether new files should be immediately uploaded to the server.
    /// </summary>
    [JsonPropertyName("instantUpload")]
    public bool InstantUpload { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether chunked uploads are enabled.
    /// When enabled, files will be automatically cut up into chunks before upload.
    /// </summary>
    [JsonPropertyName("chunkUploads")]
    public bool ChunkUploads { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to force chunks even for files smaller than the set chunkSize.
    /// </summary>
    [JsonPropertyName("chunkForce")]
    public bool ChunkForce { get; set; } = false;

    /// <summary>
    /// Gets or sets the size of a chunk in bytes.
    /// </summary>
    [JsonPropertyName("chunkSize")]
    public int ChunkSize { get; set; } = 5000000;

    /// <summary>
    /// Gets or sets the amounts of times, and delays, between retried uploading of a chunk.
    /// </summary>
    [JsonPropertyName("chunkRetryDelays")]
    public List<int> ChunkRetryDelays { get; set; } =
    [
        500,
        1000,
        3000
    ];

    #region Style

    /// <summary>
    /// Gets or sets a different layout render mode. Can be either 'integrated', 'compact', and/or 'circle'.
    /// Compact mode will remove padding, integrated mode is used to render FilePond as part of a bigger element.
    /// Circle mode adjusts the item position offsets so buttons and progress indicators don't fall outside of the circular shape.
    /// </summary>
    [JsonPropertyName("stylePanelLayout")]
    public string? StylePanelLayout { get; set; } = null;

    /// <summary>
    /// Gets or sets a forced aspect ratio for the FilePond drop area.
    /// Useful to make the drop area take up a fixed amount of space. For example, when cropping a single square image.
    /// Accepts human-readable aspect ratios like '1:1' or numeric aspect ratios like 0.75.
    /// </summary>
    [JsonPropertyName("stylePanelAspectRatio")]
    public string? StylePanelAspectRatio { get; set; } = null;

    /// <summary>
    /// Gets or sets the position of the remove item button, 'left', 'center', 'right', and/or 'bottom'.
    /// </summary>
    [JsonPropertyName("styleButtonRemoveItemPosition")]
    public string StyleButtonRemoveItemPosition { get; set; } = "left";

    /// <summary>
    /// Gets or sets the position of the process item button, 'left', 'center', 'right', and/or 'bottom'.
    /// </summary>
    [JsonPropertyName("styleButtonProcessItemPosition")]
    public string StyleButtonProcessItemPosition { get; set; } = "right";

    /// <summary>
    /// Gets or sets the position of the load indicator, 'left', 'center', 'right', and/or 'bottom'.
    /// </summary>
    [JsonPropertyName("styleLoadIndicatorPosition")]
    public string StyleLoadIndicatorPosition { get; set; } = "right";

    /// <summary>
    /// Gets or sets the position of the progress indicator, 'left', 'center', 'right', and/or 'bottom'.
    /// </summary>
    [JsonPropertyName("styleProgressIndicatorPosition")]
    public string StyleProgressIndicatorPosition { get; set; } = "right";

    /// <summary>
    /// Gets or sets a forced aspect ratio for the file items.
    /// Useful when rendering cropped or fixed aspect ratio images in grid view, this will improve performance
    /// as FilePond will know beforehand the size of the item to render.
    /// </summary>
    [JsonPropertyName("styleItemPanelAspectRatio")]
    public string? StyleItemPanelAspectRatio { get; set; } = null;

    #endregion Style

    #region Icons

    /// <summary>
    /// The icon used for remove actions. (SVG)
    /// </summary>
    [JsonPropertyName("iconRemove")]
    public string? IconRemove { get; set; }

    /// <summary>
    /// The icon used for process actions. (SVG)
    /// </summary>
    [JsonPropertyName("iconProcess")]
    public string? IconProcess { get; set; }

    /// <summary>
    /// The icon used for retry actions. (SVG)
    /// </summary>
    [JsonPropertyName("iconRetry")]
    public string? IconRetry { get; set; }

    /// <summary>
    /// The icon used for undo actions. (SVG)
    /// </summary>
    [JsonPropertyName("iconUndo")]
    public string? IconUndo { get; set; }

    #endregion Icons

    #region Plugin - FileEncode

    /// <summary>
    /// Enable or disable file encoding.
    /// </summary>
    [JsonPropertyName("allowFileEncode")]
    public bool AllowFileEncode { get; set; } = true;

    #endregion Plugin - FileEncode

    #region Plugin - FileMetadata

    /// <summary>
    /// Enable or disable the file metadata object.
    /// </summary>
    [JsonPropertyName("allowFileMetadata")]
    public bool AllowFileMetadata { get; set; } = true;

    /// <summary>
    /// The object that is used to set the initial metadata object of each file item.
    /// </summary>
    [JsonPropertyName("fileMetadataObject")]
    public object? FileMetadataObject { get; set; } = null;

    #endregion Plugin - FileMetadata

    #region Plugin - FilePoster

    /// <summary>
    /// Enable or disable the file poster functionality.
    /// </summary>
    [JsonPropertyName("allowFilePoster")]
    public bool AllowFilePoster { get; set; } = true;

    /// <summary>
    /// Minimum poster height.
    /// </summary>
    [JsonPropertyName("filePosterMinHeight")]
    public int? FilePosterMinHeight { get; set; } = null;

    /// <summary>
    /// Maximum poster height.
    /// </summary>
    [JsonPropertyName("filePosterMaxHeight")]
    public int? FilePosterMaxHeight { get; set; } = null;

    /// <summary>
    /// Fixed poster height, overrides min and max preview height.
    /// </summary>
    [JsonPropertyName("filePosterHeight")]
    public int? FilePosterHeight { get; set; } = null;

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
    /// <summary>
    /// Receives file item, return true to ignore the file.
    /// </summary>
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
    //[JsonPropertyName("filePosterFilterItem")]
    //public Func<bool> FilePosterFilterItem { get; set; } = () => true;

    /// <summary>
    /// Cross-origin value to set to the poster image element.
    /// </summary>
    [JsonPropertyName("filePosterCrossOriginAttributeValue")]
    public string FilePosterCrossOriginAttributeValue { get; set; } = "Anonymous";

    /// <summary>
    /// Three RGB values between 0 and 255 to determine shadow color, [255, 0, 0].
    /// </summary>
    [JsonPropertyName("filePosterItemOverlayShadowColor")]
    public int[]? FilePosterItemOverlayShadowColor { get; set; } = null;

    /// <summary>
    /// Three RGB values between 0 and 255 to determine error color, [255, 0, 0].
    /// </summary>
    [JsonPropertyName("filePosterItemOverlayErrorColor")]
    public int[]? FilePosterItemOverlayErrorColor { get; set; } = null;

    /// <summary>
    /// Three RGB values between 0 and 255 to determine success color, [255, 0, 0].
    /// </summary>
    [JsonPropertyName("filePosterItemOverlaySuccessColor")]
    public int[]? FilePosterItemOverlaySuccessColor { get; set; } = null;

    #endregion Plugin - FilePoster

    #region Plugin - FileRename

    /// <summary>
    /// Enable or disable file renaming.
    /// </summary>
    [JsonPropertyName("allowFileRename")]
    public bool AllowFileRename { get; set; } = true;

    #endregion Plugin - FileRename

    #region Plugin - FileSizeValidation

    /// <summary>
    /// Gets or sets a value indicating whether to allow file size validation.
    /// </summary>
    [JsonPropertyName("allowFileSizeValidation")]
    public bool AllowFileSizeValidation { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum size of a file, for instance 5MB or 750KB.
    /// </summary>
    [JsonPropertyName("minFileSize")]
    public long? MinFileSize { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum size of a file, for instance 5MB or 750KB.
    /// </summary>
    [JsonPropertyName("maxFileSize")]
    public long? MaxFileSize { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum size of all files in the list, in the same format as maxFileSize.
    /// </summary>
    [JsonPropertyName("maxTotalFileSize")]
    public long? MaxTotalFileSize { get; set; } = null;

    /// <summary>
    /// Gets or sets the status message shown when a large file is dropped and exceeds the maximum file size.
    /// </summary>
    [JsonPropertyName("labelMaxFileSizeExceeded")]
    public string LabelMaxFileSizeExceeded { get; set; } = "File is too large";

    /// <summary>
    /// Gets or sets the detail message shown when the max file size is exceeded.
    /// {filesize} is replaced with the value of the maxFileSize property.
    /// </summary>
    [JsonPropertyName("labelMaxFileSize")]
    public string LabelMaxFileSize { get; set; } = "Maximum file size is {filesize}";

    /// <summary>
    /// Gets or sets the status message shown when the total file size exceeds the maximum.
    /// </summary>
    [JsonPropertyName("labelMaxTotalFileSizeExceeded")]
    public string LabelMaxTotalFileSizeExceeded { get; set; } = "Maximum total size exceeded";

    /// <summary>
    /// Gets or sets the detail message shown when the total file size exceeds the maximum.
    /// {filesize} is replaced with the value of the maxTotalFileSize property.
    /// </summary>
    [JsonPropertyName("labelMaxTotalFileSize")]
    public string LabelMaxTotalFileSize { get; set; } = "Maximum total file size is {filesize}";

    #endregion Plugin - FileSizeValidation

    #region Plugin - FileTypeValidation

    /// <summary>
    /// Enable or disable file type validation.
    /// </summary>
    [JsonPropertyName("allowFileTypeValidation")]
    public bool AllowFileTypeValidation { get; set; } = true;

    /// <summary>
    /// Array of accepted file types. Can be mime types or wild cards.
    /// For instance ['image/*'] will accept all images. ['image/png', 'image/jpeg']
    /// will only accept PNGs and JPEGs.
    /// </summary>
    [JsonPropertyName("acceptedFileTypes")]
    public List<string> AcceptedFileTypes { get; set; } = [];

    /// <summary>
    /// Message shown when an invalid file is added.
    /// </summary>
    [JsonPropertyName("labelFileTypeNotAllowed")]
    public string LabelFileTypeNotAllowed { get; set; } = "File of invalid type";

    /// <summary>
    /// Message shown to indicate the allowed file types.
    /// Available placeholders are {allTypes}, {allButLastType}, {lastType}.
    /// </summary>
    [JsonPropertyName("fileValidateTypeLabelExpectedTypes")]
    public string FileValidateTypeLabelExpectedTypes { get; set; } = "Expects {allButLastType} or {lastType}";

    /// <summary>
    /// Allows mapping the file type to a more visually appealing label.
    /// { 'image/jpeg': '.jpg' } will show .jpg in the expected types label.
    /// Set to null to hide a type from the label.
    /// </summary>
    [JsonPropertyName("fileValidateTypeLabelExpectedTypesMap")]
    public Dictionary<string, string> FileValidateTypeLabelExpectedTypesMap { get; set; } = [];

    #endregion Plugin - FileTypeValidation

    #region Plugin - ImageCrop

    /// <summary>
    /// Enable or disable image cropping.
    /// </summary>
    [JsonPropertyName("allowImageCrop")]
    public bool AllowImageCrop { get; set; } = true;

    /// <summary>
    /// The aspect ratio of the crop in human-readable format, for example '1:1' or '16:10'.
    /// </summary>
    [JsonPropertyName("imageCropAspectRatio")]
    public string? ImageCropAspectRatio { get; set; } = null;

    #endregion Plugin - ImageCrop

    #region Plugin - ImageEdit

    /// <summary>
    /// Enable or disable image editing.
    /// </summary>
    [JsonPropertyName("allowImageEdit")]
    public bool AllowImageEdit { get; set; } = true;

    /// <summary>
    /// Position of the image edit button within the image preview window.
    /// </summary>
    [JsonPropertyName("styleImageEditButtonEditItemPosition")]
    public string StyleImageEditButtonEditItemPosition { get; set; } = "bottom center";

    /// <summary>
    /// Instantly opens the editor when an image is added. When editing is cancelled, the image is not added to FilePond.
    /// </summary>
    [JsonPropertyName("imageEditInstantEdit")]
    public bool ImageEditInstantEdit { get; set; } = false;

    /// <summary>
    /// Disables the manual edit button.
    /// </summary>
    [JsonPropertyName("imageEditAllowEdit")]
    public bool ImageEditAllowEdit { get; set; } = true;

    /// <summary>
    /// The SVG icon used in the image edit button.
    /// </summary>
    [JsonPropertyName("imageEditIconEdit")]
    public string ImageEditIconEdit { get; set; } = "<svg></svg>";

    /// <summary>
    /// The Image Editor to link to FilePond.
    /// </summary>
    [JsonPropertyName("imageEditEditor")]
    public object? ImageEditEditor { get; set; } = null;

    #endregion Plugin - ImageEdit

    #region Plugin - ImageExifOrientation

    /// <summary>
    /// Enable or disable fetching of EXIF orientation.
    /// </summary>
    [JsonPropertyName("allowImageExifOrientation")]
    public bool AllowImageExifOrientation { get; set; } = true;

    #endregion Plugin - ImageExifOrientation

    #region Plugin - ImageFilter

    /// <summary>
    /// Enable or disable image filtering.
    /// </summary>
    [JsonPropertyName("allowImageFilter")]
    public bool AllowImageFilter { get; set; } = true;

    /// <summary>
    /// The Color Matrix to apply to the image in the preview and transform phase.
    /// </summary>
    [JsonPropertyName("imageFilterColorMatrix")]
    public object? ImageFilterColorMatrix { get; set; } = null;

    #endregion Plugin - ImageFilter

    #region Plugin - ImagePreview

    /// <summary>
    /// Gets or sets a value indicating whether image preview is enabled.
    /// </summary>
    [JsonPropertyName("allowImagePreview")]
    public bool AllowImagePreview { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum height for image preview.
    /// </summary>
    [JsonPropertyName("imagePreviewMinHeight")]
    public int ImagePreviewMinHeight { get; set; } = 44;

    /// <summary>
    /// Gets or sets the maximum height for image preview.
    /// </summary>
    [JsonPropertyName("imagePreviewMaxHeight")]
    public int ImagePreviewMaxHeight { get; set; } = 256;

    /// <summary>
    /// Gets or sets the fixed image preview height, overriding min and max preview height.
    /// </summary>
    [JsonPropertyName("imagePreviewHeight")]
    public int? ImagePreviewHeight { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum file size for image preview.
    /// Can be used to prevent loading of large images when createImageBitmap is not supported.
    /// Expects a string, like "2MB" or "500KB". By default, no maximum file size is defined.
    /// </summary>
    [JsonPropertyName("imagePreviewMaxFileSize")]
    public string? ImagePreviewMaxFileSize { get; set; } = null;

    /// <summary>
    /// Gets or sets the transparency indicator for image preview.
    /// Set to 'grid' to render a transparency grid behind the image, or set to a color value (e.g., '#f00') to set a transparent image background color.
    /// This is only for preview purposes and is not embedded in the output image.
    /// </summary>
    [JsonPropertyName("imagePreviewTransparencyIndicator")]
    public string? ImagePreviewTransparencyIndicator { get; set; } = null;

    /// <summary>
    /// Gets or sets the maximum file size for images to preview immediately.
    /// If files are larger and the browser doesn't support createImageBitmap, the preview is queued until FilePond is in a rest state.
    /// </summary>
    [JsonPropertyName("imagePreviewMaxInstantPreviewFileSize")]
    public int ImagePreviewMaxInstantPreviewFileSize { get; set; } = 1000000;

    /// <summary>
    /// Gets or sets a value indicating whether to show image markup in the preview panel.
    /// </summary>
    [JsonPropertyName("imagePreviewMarkupShow")]
    public bool ImagePreviewMarkupShow { get; set; } = true;

#pragma warning disable CS1587 // XML comment is not placed on a valid language element
    /// <summary>
    /// Gets or sets the filter for image markup items.
    /// Use to show only certain items and hide others until the image file is generated by the image transform plugin.
    /// </summary>
    //[JsonPropertyName("imagePreviewMarkupFilter")]
    //public Func<object, bool> ImagePreviewMarkupFilter { get; set; } = (markupItem) => true;

    /// <summary>
    /// Gets or sets the filter for file items before generating the preview.
    /// Use to filter certain image types or names if you do not wish to generate a preview.
    /// </summary>
#pragma warning restore CS1587 // XML comment is not placed on a valid language element
    //[JsonPropertyName("imagePreviewFilterItem")]
    //public Func<object, bool> ImagePreviewFilterItem { get; set; } = (fileItem) => true;

    #endregion Plugin - ImagePreview

    #region Plugin - ImageResize

    /// <summary>
    /// Enable or disable image resizing.
    /// </summary>
    [JsonPropertyName("allowImageResize")]
    public bool AllowImageResize { get; set; } = true;

    /// <summary>
    /// The output width in pixels, if null will use the value of imageResizeTargetHeight.
    /// </summary>
    [JsonPropertyName("imageResizeTargetWidth")]
    public int? ImageResizeTargetWidth { get; set; } = null;

    /// <summary>
    /// The output height in pixels, if null will use the value of imageResizeTargetWidth.
    /// </summary>
    [JsonPropertyName("imageResizeTargetHeight")]
    public int? ImageResizeTargetHeight { get; set; } = null;

    /// <summary>
    /// The method in which the images are resized. Choose between 'force', 'cover', or 'contain'.
    /// Force will ignore the image aspect ratio. Cover will respect the aspect ratio and will scale to fill the target dimensions.
    /// Contain also respects the aspect ratio and will fit the image inside the set dimensions.
    /// All three settings will upscale images when there are smaller than the given target dimensions.
    /// </summary>
    [JsonPropertyName("imageResizeMode")]
    public string ImageResizeMode { get; set; } = "cover";

    /// <summary>
    /// Set to false to prevent upscaling of images smaller than the target size.
    /// </summary>
    [JsonPropertyName("imageResizeUpscale")]
    public bool ImageResizeUpscale { get; set; } = true;

    #endregion Plugin - ImageResize

    #region Plugin - ImageTransform

    /// <summary>
    /// Enable or disable client-side image transforms.
    /// </summary>
    [JsonPropertyName("allowImageTransform")]
    public bool AllowImageTransform { get; set; } = true;

    /// <summary>
    /// The file type of the output image. Can be either 'image/jpeg' or 'image/png'.
    /// If not defined, will default to the input file type, and fallback to 'image/jpeg'.
    /// </summary>
    [JsonPropertyName("imageTransformOutputMimeType")]
    public string? ImageTransformOutputMimeType { get; set; } = null;

    /// <summary>
    /// The quality of the output image supplied as a value between 0 and 100.
    /// Where 100 is the best quality and 0 is the worst.
    /// When not supplied, it will use the browser default quality, which averages around 94.
    /// </summary>
    [JsonPropertyName("imageTransformOutputQuality")]
    public int? ImageTransformOutputQuality { get; set; } = null;

    /// <summary>
    /// Should output quality be enforced, set to 'optional' to apply only when a transform is required due to other requirements (e.g. resize or crop).
    /// </summary>
    [JsonPropertyName("imageTransformOutputQualityMode")]
    public string ImageTransformOutputQualityMode { get; set; } = "always";

    /// <summary>
    /// Should JPEG EXIF data be stripped from the output image, defaults to true (as that is what the browser does).
    /// Set to false to copy over the EXIF data from the original image to the output image.
    /// This will automatically remove the EXIF orientation tag to prevent orientation problems.
    /// </summary>
    [JsonPropertyName("imageTransformOutputStripImageHead")]
    public bool ImageTransformOutputStripImageHead { get; set; } = true;

    /// <summary>
    /// An array of transforms to apply on the client, useful if we, for instance, want to do resizing on the client but cropping on the server.
    /// null means apply all transforms ('resize', 'crop').
    /// </summary>
    [JsonPropertyName("imageTransformClientTransforms")]
    public string[]? ImageTransformClientTransforms { get; set; } = null;

    /// <summary>
    /// An object that can be used to output multiple different files based on different transform instructions.
    /// </summary>
    [JsonPropertyName("imageTransformVariants")]
    public object? ImageTransformVariants { get; set; } = null;

    /// <summary>
    /// Should the transform plugin output the default transformed file.
    /// </summary>
    [JsonPropertyName("imageTransformVariantsIncludeDefault")]
    public bool ImageTransformVariantsIncludeDefault { get; set; } = true;

    /// <summary>
    /// The name to use in front of the file name for the default transformed file.
    /// </summary>
    [JsonPropertyName("imageTransformVariantsDefaultName")]
    public string? ImageTransformVariantsDefaultName { get; set; } = null;

    /// <summary>
    /// Should the transform plugin output the original file.
    /// </summary>
    [JsonPropertyName("imageTransformVariantsIncludeOriginal")]
    public bool ImageTransformVariantsIncludeOriginal { get; set; } = false;

    /// <summary>
    /// The name to use in front of the original file name for the original file.
    /// </summary>
    [JsonPropertyName("imageTransformVariantsOriginalName")]
    public string? ImageTransformVariantsOriginalName { get; set; } = null;

    /// <summary>
    /// A hook to make changes to the canvas before the file is created.
    /// </summary>
    [JsonPropertyName("imageTransformBeforeCreateBlob")]
    public object? ImageTransformBeforeCreateBlob { get; set; } = null;

    /// <summary>
    /// A hook to make changes to the file after the file has been created.
    /// </summary>
    [JsonPropertyName("imageTransformAfterCreateBlob")]
    public object? ImageTransformAfterCreateBlob { get; set; } = null;

    /// <summary>
    /// A memory limit to make sure the canvas can be used correctly when rendering the image.
    /// By default, this is only active on iOS.
    /// </summary>
    [JsonPropertyName("imageTransformCanvasMemoryLimit")]
    public object? ImageTransformCanvasMemoryLimit { get; set; } = null;

    #endregion Plugin - ImageTransform

    #region Plugin - ImageValidateSize

    /// <summary>
    /// Enable or disable image size validation.
    /// </summary>
    [JsonPropertyName("allowImageValidateSize")]
    public bool AllowImageValidateSize { get; set; } = true;

    /// <summary>
    /// The minimum image width.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMinWidth")]
    public int ImageValidateSizeMinWidth { get; set; } = 1;

    /// <summary>
    /// The maximum image width.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMaxWidth")]
    public int ImageValidateSizeMaxWidth { get; set; } = 65535;

    /// <summary>
    /// The minimum image height.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMinHeight")]
    public int ImageValidateSizeMinHeight { get; set; } = 1;

    /// <summary>
    /// The maximum image height.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMaxHeight")]
    public int ImageValidateSizeMaxHeight { get; set; } = 65535;

    /// <summary>
    /// The message shown when the image is not supported by the browser.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelFormatError")]
    public string ImageValidateSizeLabelFormatError { get; set; } = "Image type not supported";

    /// <summary>
    /// The message shown when the image is too small.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelImageSizeTooSmall")]
    public string ImageValidateSizeLabelImageSizeTooSmall { get; set; } = "Image is too small";

    /// <summary>
    /// The message shown when the image is too big.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelImageSizeTooBig")]
    public string ImageValidateSizeLabelImageSizeTooBig { get; set; } = "Image is too big";

    /// <summary>
    /// Message shown to indicate the minimum image size.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelExpectedMinSize")]
    public string ImageValidateSizeLabelExpectedMinSize { get; set; } = "Minimum size is {minWidth} × {minHeight}";

    /// <summary>
    /// Message shown to indicate the maximum image size.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelExpectedMaxSize")]
    public string ImageValidateSizeLabelExpectedMaxSize { get; set; } = "Maximum size is {maxWidth} × {maxHeight}";

    /// <summary>
    /// The minimum image resolution.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMinResolution")]
    public int? ImageValidateSizeMinResolution { get; set; } = null;

    /// <summary>
    /// The maximum image resolution.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMaxResolution")]
    public int? ImageValidateSizeMaxResolution { get; set; } = null;

    /// <summary>
    /// The message shown when the image resolution is too low.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelImageResolutionTooLow")]
    public string ImageValidateSizeLabelImageResolutionTooLow { get; set; } = "Resolution is too low";

    /// <summary>
    /// The message shown when the image resolution is too high.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelImageResolutionTooHigh")]
    public string ImageValidateSizeLabelImageResolutionTooHigh { get; set; } = "Resolution is too high";

    /// <summary>
    /// Message shown to indicate the minimum image resolution.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelExpectedMinResolution")]
    public string ImageValidateSizeLabelExpectedMinResolution { get; set; } = "Minimum resolution is {minResolution}";

    /// <summary>
    /// Message shown to indicate the maximum image resolution.
    /// </summary>
    [JsonPropertyName("imageValidateSizeLabelExpectedMaxResolution")]
    public string ImageValidateSizeLabelExpectedMaxResolution { get; set; } = "Maximum resolution is {maxResolution}";

    /// <summary>
    /// A custom function to measure the image file, for when you want to measure image formats not supported by browsers.
    /// Receives the image file, should return a Promise. Resolve should pass a size object containing both a width and height parameter.
    /// Reject should be called if the image format is not supported / can't be measured.
    /// </summary>
    [JsonPropertyName("imageValidateSizeMeasure")]
    public object? ImageValidateSizeMeasure { get; set; } = null;

    #endregion Plugin - ImageValidateSize
}