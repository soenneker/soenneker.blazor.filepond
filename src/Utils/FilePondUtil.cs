using Soenneker.Blazor.FilePond.Enums;
using System;

namespace Soenneker.Blazor.FilePond.Utils;

internal class FilePondUtil
{
    public static (string uri, string integrity) GetUriAndIntegrityForStyle(FilePondPluginType? type)
    {
        if (type == null)
            return ("https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.css", "sha256-a95jYCBL4++k1XyLYgulKmY33bIJIVYMsJO/RNytaJM=");

        switch (type.Name)
        {
            case nameof(FilePondPluginType.ImagePreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.css", "sha256-YsO8aMI20vSizdmc2mmWx2DU1wof4v60Nwy7hIBvdM8=");
        }

        throw new NotSupportedException();
    }

    public static (string uri, string integrity) GetUriAndIntegrityForScript(FilePondPluginType? type)
    {
        if (type == null)
            return ("https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.js", "sha256-6yXpr8+sATA4Q2ANTyZmpn4ZGP7grbIRNpe9s0Y+iO0=");

        switch (type.Name)
        {
            case nameof(FilePondPluginType.ImagePreview):
                return ("https://cdn.jsdelivr.net/npm/filepond-plugin-image-preview@4.6.12/dist/filepond-plugin-image-preview.min.js", "sha256-1vQHytMrpOsKFyUUPuEVhxeNH6UhMX3uNetDRvlSpwU=");
        }

        throw new NotSupportedException();
    }
}