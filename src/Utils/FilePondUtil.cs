using Soenneker.Blazor.FilePond.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Soenneker.Blazor.FilePond.Utils;

internal class FilePondUtil
{
    public static (string? uri, string? integrity) GetUriAndIntegrityForStyle(bool useCdn = true, FilePondPluginType? type = null,
        string localBasePath = "_content/Soenneker.Blazor.FilePond/css/")
    {
        if (type == null)
        {
            return useCdn
                ? ("https://cdn.jsdelivr.net/npm/filepond@4.32.7/dist/filepond.min.css", "sha256-R/TKiFR8YXiqvCSFSm3ek/rIjgEoFS5PpaAMkv/brg4=")
                : ($"{localBasePath}filepond.min.css", null);
        }

        var cdnLinks = new Dictionary<string, (string uri, string integrity)>
        {
            {
                nameof(FilePondPluginType.ImagePreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.css",
                    "sha256-YsO8aMI20vSizdmc2mmWx2DU1wof4v60Nwy7hIBvdM8=")
            },
            {
                nameof(FilePondPluginType.ImageOverlay),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-overlay@1.0.9/dist/filepond-plugin-image-overlay.min.css",
                    "sha256-yqH8PFISV0ZhpMB6AgjfQtGfyZRMxwMH2bdzA3gxJ2s=")
            },
            {
                nameof(FilePondPluginType.MediaPreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-media-preview@1.0.11/dist/filepond-plugin-media-preview.min.css",
                    "sha256-VQqmcXKFOgNliUlsMiPGaocGwCY0AVH88lAs4Gt9/WM=")
            },
            {
                nameof(FilePondPluginType.ImageEdit),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-edit@1.6.3/dist/filepond-plugin-image-edit.min.css",
                    "sha256-8pWIaKtSSlpautzbSUOi8F/WHhVvbfByRkDjUff6hZg=")
            },
            {
                nameof(FilePondPluginType.FilePoster),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-poster@2.5.1/dist/filepond-plugin-file-poster.min.css",
                    "sha256-rX6jcSTtxygcIKjtgousEYPuZlyvaLRXumGXjGBdzDw=")
            },
            {
                nameof(FilePondPluginType.PdfPreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-pdf-preview@1.0.4/dist/filepond-plugin-pdf-preview.min.css",
                    "sha256-8hpqdbd5csscLTdyKZpnl3Kkf7EUPQCx6RbIHu0KTcU=")
            }
        };

        if (cdnLinks.TryGetValue(type.Name, out (string uri, string integrity) cdnData))
        {
            string fileName = cdnData.uri.Split('/').Last(); // Extract filename from CDN URL
            return useCdn ? cdnData : ($"{localBasePath}{fileName}", null);
        }

        return (null, null);
    }

    public static (string? uri, string? integrity) GetUriAndIntegrityForScript(bool useCdn = true, FilePondPluginType? type = null,
        string localBasePath = "_content/Soenneker.Blazor.FilePond/js/")
    {
        if (type == null)
        {
            return useCdn
                ? ("https://cdn.jsdelivr.net/npm/filepond@4.32.7/dist/filepond.min.js", "sha256-BRICH2AsAT7Vx36hU5PcHTuKBbusAU4j6fge+/dHO1M=")
                : ($"{localBasePath}filepond.min.js", null);
        }

        var cdnLinks = new Dictionary<string, (string uri, string integrity)>
        {
            {
                nameof(FilePondPluginType.ImagePreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.js",
                    "sha256-1vQHytMrpOsKFyUUPuEVhxeNH6UhMX3uNetDRvlSpwU=")
            },
            {
                nameof(FilePondPluginType.FileValidateType),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-validate-type@1.2.9/dist/filepond-plugin-file-validate-type.min.js",
                    "sha256-iNzotay9f+s57lx/JEVaRWbOsGGXZlPPZlSQ34OHx+A=")
            },
            {
                nameof(FilePondPluginType.FileValidateSize),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-validate-size@2.2.8/dist/filepond-plugin-file-validate-size.min.js",
                    "sha256-XaHceW4NII52xG26aOw+77JG8yEk95GAZs8h7ZAv1xo=")
            },
            {
                nameof(FilePondPluginType.ImageExifOrientation),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-exif-orientation@1.0.11/dist/filepond-plugin-image-exif-orientation.min.js",
                    "sha256-mFhzQjlQ9XmXxDi70Pv4uxRUQjQkkIrICcHq1k2PyzQ=")
            },
            {
                nameof(FilePondPluginType.ImageOverlay),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-overlay@1.0.9/dist/filepond-plugin-image-overlay.min.js",
                    "sha256-VjaMjvbV/ohMNyRbuephi6Vf6AogNEJ2ZdTJkOrwgwc=")
            },
            {
                nameof(FilePondPluginType.FileRename),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-rename@1.1.8/dist/filepond-plugin-file-rename.min.js",
                    "sha256-exIcwK39cEV8VNevRX14rnvcKrRDAKsmb3OX2FZxSx4=")
            },
            {
                nameof(FilePondPluginType.FileMetadata),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-metadata@1.0.8/dist/filepond-plugin-file-metadata.min.js",
                    "sha256-2HwMKXUiYKc7qxWB9fLzJBbKZpdRpfzBKh2PmqRftUs=")
            },
            {
                nameof(FilePondPluginType.MediaPreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-media-preview@1.0.11/dist/filepond-plugin-media-preview.min.js",
                    "sha256-xf3Kv3/gITsdJ04Nwrsc/hY187RCnVzGsYTDXiUv4Ac=")
            },
            {
                nameof(FilePondPluginType.FileEncode),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-encode@2.1.14/dist/filepond-plugin-file-encode.min.js",
                    "sha256-5SCgOnPI7R97+Kki++DXFpTBw+u9LfjLeuRmlnuk+uc=")
            },
            {
                nameof(FilePondPluginType.ImageTransform),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-transform@3.8.7/dist/filepond-plugin-image-transform.min.js",
                    "sha256-GU0jEm8SoiNKHfsqyKpkOea4WkH3g9wmZtfndENkkm4=")
            },
            {
                nameof(FilePondPluginType.ImageCrop),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-crop@2.0.6/dist/filepond-plugin-image-crop.min.js",
                    "sha256-pjptfDhJ5LHFJpWf/tnkvgJBELYDk6m9cG7gv+IFaII=")
            },
            {
                nameof(FilePondPluginType.ImageResize),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-resize@2.0.10/dist/filepond-plugin-image-resize.min.js",
                    "sha256-OBxJKil/uWeV/37M0UgZT5UsJh/lQlslOcN0dDWmzso=")
            },
            {
                nameof(FilePondPluginType.ImageEdit),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-edit@1.6.3/dist/filepond-plugin-image-edit.min.js",
                    "sha256-3Uo0QE2ImY3ODnVXhRju32Jo6TiI1vkblEv+p6rzsBY=")
            },
            {
                nameof(FilePondPluginType.FilePoster),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-file-poster@2.5.1/dist/filepond-plugin-file-poster.min.js",
                    "sha256-ujyZGxQynUBTF7MOO3m9BJb6RRmn0DVwTlpBOzTU7cM=")
            },
            {
                nameof(FilePondPluginType.ImageValidateSize),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-validate-size@1.2.7/dist/filepond-plugin-image-validate-size.min.js",
                    "sha256-JWnTuAHJtBLPX14J6GO6DTszR64pW0Dk9Vnim8qv1Zc=")
            },
            {
                nameof(FilePondPluginType.ImageFilter),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-filter@1.0.1/dist/filepond-plugin-image-filter.min.js",
                    "sha256-8mJwlLEcsBJGc849v9g6D2amkclGo7dpsvmpZoYtGRw=")
            },
            {
                nameof(FilePondPluginType.PdfPreview),
                ("https://cdn.jsdelivr.net/npm/filepond-plugin-pdf-preview@1.0.4/dist/filepond-plugin-pdf-preview.min.js",
                    "sha256-7pKAlLIFOrszavQZHrDtpHk4YBb090kZ+YY/iIWHhN0=")
            }
        };

        if (cdnLinks.TryGetValue(type.Name, out (string uri, string integrity) cdnData))
        {
            string fileName = cdnData.uri.Split('/').Last(); // Extract filename from CDN URL
            return useCdn ? cdnData : ($"{localBasePath}{fileName}", null);
        }

        return (null, null);
    }
}