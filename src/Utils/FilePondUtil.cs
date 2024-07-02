using Soenneker.Blazor.FilePond.Enums;

namespace Soenneker.Blazor.FilePond.Utils;

internal class FilePondUtil
{
    public static (string? uri, string? integrity) GetUriAndIntegrityForStyle(FilePondPluginType? type)
    {
        if (type == null)
            return ("https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.css", "sha256-a95jYCBL4++k1XyLYgulKmY33bIJIVYMsJO/RNytaJM=");

        switch (type.Name)
        {
            case nameof(FilePondPluginType.ImagePreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.css", "sha256-YsO8aMI20vSizdmc2mmWx2DU1wof4v60Nwy7hIBvdM8=");
            case nameof(FilePondPluginType.ImageOverlay):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-overlay@1.0.9/dist/filepond-plugin-image-overlay.min.css", "sha256-yqH8PFISV0ZhpMB6AgjfQtGfyZRMxwMH2bdzA3gxJ2s=");
            case nameof(FilePondPluginType.MediaPreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-media-preview@1.0.11/dist/filepond-plugin-media-preview.min.css", "sha256-VQqmcXKFOgNliUlsMiPGaocGwCY0AVH88lAs4Gt9/WM=");
            case nameof(FilePondPluginType.ImageEdit):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-edit@1.6.3/dist/filepond-plugin-image-edit.min.css", "sha256-8pWIaKtSSlpautzbSUOi8F/WHhVvbfByRkDjUff6hZg=");
            case nameof(FilePondPluginType.FilePoster):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-poster@2.5.1/dist/filepond-plugin-file-poster.min.css", "sha256-rX6jcSTtxygcIKjtgousEYPuZlyvaLRXumGXjGBdzDw=");
            case nameof(FilePondPluginType.PdfPreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-pdf-preview@1.0.4/dist/filepond-plugin-pdf-preview.min.css", "sha256-8hpqdbd5csscLTdyKZpnl3Kkf7EUPQCx6RbIHu0KTcU=");
        }

        return (null, null);
    }

    public static (string? uri, string? integrity) GetUriAndIntegrityForScript(FilePondPluginType? type)
    {
        if (type == null)
            return ("https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.js", "sha256-6yXpr8+sATA4Q2ANTyZmpn4ZGP7grbIRNpe9s0Y+iO0=");

        switch (type.Name)
        {
            case nameof(FilePondPluginType.ImagePreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.js", "sha256-1vQHytMrpOsKFyUUPuEVhxeNH6UhMX3uNetDRvlSpwU=");
            case nameof(FilePondPluginType.FileValidateType):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-validate-type@1.2.9/dist/filepond-plugin-file-validate-type.min.js", "sha256-iNzotay9f+s57lx/JEVaRWbOsGGXZlPPZlSQ34OHx+A=");
            case nameof(FilePondPluginType.FileValidateSize):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-validate-size@2.2.8/dist/filepond-plugin-file-validate-size.min.js", "sha256-XaHceW4NII52xG26aOw+77JG8yEk95GAZs8h7ZAv1xo=");
            case nameof(FilePondPluginType.ImageExifOrientation):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-exif-orientation@1.0.11/dist/filepond-plugin-image-exif-orientation.min.js", "sha256-mFhzQjlQ9XmXxDi70Pv4uxRUQjQkkIrICcHq1k2PyzQ=");
            case nameof(FilePondPluginType.ImageOverlay):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-overlay@1.0.9/dist/filepond-plugin-image-overlay.min.js", "sha256-VjaMjvbV/ohMNyRbuephi6Vf6AogNEJ2ZdTJkOrwgwc=");
            case nameof(FilePondPluginType.FileRename):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-rename@1.1.8/dist/filepond-plugin-file-rename.min.js", "sha256-exIcwK39cEV8VNevRX14rnvcKrRDAKsmb3OX2FZxSx4=");
            case nameof(FilePondPluginType.FileMetadata):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-metadata@1.0.8/dist/filepond-plugin-file-metadata.min.js", "sha256-2HwMKXUiYKc7qxWB9fLzJBbKZpdRpfzBKh2PmqRftUs=");
            case nameof(FilePondPluginType.MediaPreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-media-preview@1.0.11/dist/filepond-plugin-media-preview.min.js", "sha256-xf3Kv3/gITsdJ04Nwrsc/hY187RCnVzGsYTDXiUv4Ac=");
            case nameof(FilePondPluginType.FileEncode):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-encode@2.1.14/dist/filepond-plugin-file-encode.min.js", "sha256-5SCgOnPI7R97+Kki++DXFpTBw+u9LfjLeuRmlnuk+uc=");
            case nameof(FilePondPluginType.ImageTransform):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-transform@3.8.7/dist/filepond-plugin-image-transform.min.js", "sha256-GU0jEm8SoiNKHfsqyKpkOea4WkH3g9wmZtfndENkkm4=");
            case nameof(FilePondPluginType.ImageCrop):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-crop@2.0.6/dist/filepond-plugin-image-crop.min.js", "sha256-pjptfDhJ5LHFJpWf/tnkvgJBELYDk6m9cG7gv+IFaII=");
            case nameof(FilePondPluginType.ImageResize):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-resize@2.0.10/dist/filepond-plugin-image-resize.min.js", "sha256-OBxJKil/uWeV/37M0UgZT5UsJh/lQlslOcN0dDWmzso=");
            case nameof(FilePondPluginType.ImageEdit):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-edit@1.6.3/dist/filepond-plugin-image-edit.min.js", "sha256-3Uo0QE2ImY3ODnVXhRju32Jo6TiI1vkblEv+p6rzsBY=");
            case nameof(FilePondPluginType.FilePoster):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-poster@2.5.1/dist/filepond-plugin-file-poster.min.js", "sha256-ujyZGxQynUBTF7MOO3m9BJb6RRmn0DVwTlpBOzTU7cM=");
            case nameof(FilePondPluginType.ImageValidateSize):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-validate-size@1.2.7/dist/filepond-plugin-image-validate-size.min.js", "sha256-JWnTuAHJtBLPX14J6GO6DTszR64pW0Dk9Vnim8qv1Zc=");
            case nameof(FilePondPluginType.ImageFilter):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-filter@1.0.1/dist/filepond-plugin-image-filter.min.js", "sha256-8mJwlLEcsBJGc849v9g6D2amkclGo7dpsvmpZoYtGRw=");
            case nameof(FilePondPluginType.PdfPreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-pdf-preview@1.0.4/dist/filepond-plugin-pdf-preview.min.js", "sha256-7pKAlLIFOrszavQZHrDtpHk4YBb090kZ+YY/iIWHhN0=");
        }

        return (null, null);
    }
}