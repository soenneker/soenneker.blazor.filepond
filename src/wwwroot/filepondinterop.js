window.filepondinterop = (function () {
    var ponds = {};
    var options = {};

    function create(elementId, options) {
        var pond;

        var selector = '[blazor-interop-id="' + elementId + '"]';
        var element = document.querySelector(selector);

        if (options) {
            var opt = JSON.parse(options);
            pond = FilePond.create(element, opt);
            options[elementId] = opt;
        }
        else
            pond = FilePond.create(element);

        ponds[elementId] = pond;
    }

    function setOptions(elementId, options) {
        var pond = ponds[elementId];

        var opt = JSON.parse(options);

        options[elementId] = opt;
        pond.setOptions(opt);
    }

    function addFile(elementId, uri, options) {
        var pond = ponds[elementId];

        if (!options)
            pond.addFile(uri);
        else
            pond.addFile(uri, options);
    }

    async function addFileFromStream(elementId, streamRef, options) {
        var pond = ponds[elementId];

        const readableStream = await streamRef.stream();

        var response = new Response(readableStream);

        try {
            response.blob().then((blob) => {
                if (!options)
                    pond.addFile(blob);
                else
                    pond.addFile(blob, options);
            });
        } catch (error) {
            console.error('Error adding file from stream:', error);
        }
    }

    function addFiles(elementId, uris, options) {
        var pond = ponds[elementId];

        if (!options)
            pond.addFiles(uris);
        else
            pond.addFiles(uris, options);
    }

    function removeFile(elementId, query) {
        var pond = ponds[elementId];

        if (!query)
            pond.removeFile();
        else
            pond.removeFile(query);
    }

    function removeFiles(elementId, query) {
        var pond = ponds[elementId];

        if (!query)
            pond.removeFiles();
        else
            pond.removeFiles(query);
    }

    function processFile(elementId, query) {
        var pond = ponds[elementId];

        if (!query)
            pond.processFile();
        else
            pond.processFile(query);

    }

    function processFiles(elementId, query) {
        var pond = ponds[elementId];

        if (!query)
            pond.processFiles();
        else
            pond.processFiles(query);
    }

    function prepareFile(elementId, query) {
        var pond = ponds[elementId];

        return pond.prepareFile(query);
    }

    function prepareFiles(elementId, query) {
        var pond = ponds[elementId];

        return pond.prepareFiles(query);
    }

    function getFile(elementId, query) {
        var pond = ponds[elementId];
        const file = pond.getFile(query);
        var result = getJsonFromObjectOrArray(file);
        return result;
    }

    function getFiles(elementId) {
        var pond = ponds[elementId];
        const files = pond.getFiles();
        var result = getJsonFromObject(files);
        return result;
    }

    function browse(elementId) {
        var pond = ponds[elementId];
        pond.browse();
    }

    function sort(elementId, compare) {
        var pond = ponds[elementId];

        var compareFn = window[compare];

        pond.getFiles().sort(compareFn);
    }

    function moveFile(elementId, query, index) {
        var pond = ponds[elementId];

        pond.moveFile(query, index);
    }

    function destroy(elementId, element) {
        var pond = ponds[elementId];

        if (pond) {
            pond.destroy();
        }

        ponds[elementId] = null;
        delete ponds[elementId];

        options[elementId] = null;
        delete options[elementId];
    }

    function addEventListener(elementId, eventName, dotNetCallback) {
        var pond = ponds[elementId];
        pond.on(eventName, function (...args) {
            var json = getJsonFromArguments(...args);

            return dotNetCallback.invokeMethodAsync("Invoke", json);
        });
    }

    function addOutputEventListener(elementId, eventName, dotNetCallback) {
        var pond = ponds[elementId];

        function handleFileEvent(file) {
            var filePondItemJson = getJsonFromObjectOrArray(file);

            return dotNetCallback.invokeMethodAsync("InvokeWithOutput", filePondItemJson).then((data) => {
                return data;
            });
        }

        function handleRenameEvent(file) {
            var json = JSON.stringify(file);

            return dotNetCallback.invokeMethodAsync("InvokeWithOutput", json).then((data) => {
                return data;
            });
        }

        switch (eventName) {
            case "OnBeforeAddFile":
                pond.beforeAddFile = function (file) {
                    return handleFileEvent(file);
                };
                break;
            case "OnBeforeDropFile":
                pond.beforeDropFile = function (file) {
                    return handleFileEvent(file);
                };
                break;
            case "OnBeforeRemoveFile":
                pond.beforeRemoveFile = function (file) {
                    return handleFileEvent(file);
                };
                break;
            case "OnFileRename":
                var existingOptions = options[elementId];

                if (!existingOptions)
                    existingOptions = {};

                existingOptions.fileRenameFunction = function (file) {
                    return handleRenameEvent(file);
                };

                pond.setOptions(existingOptions);
                options[elementId] = existingOptions;
                break;
        }
    }

    function enablePlugins(plugins) {
        plugins.forEach(plugin => {
            var pluginName = "FilePondPlugin" + plugin;

            var pluginVariable = window[pluginName];

            if (pluginVariable)
                FilePond.registerPlugin(pluginVariable);
            else
                console.error("Could not load FilePond plugin (" + pluginName + "), are you sure the necessary script is on the page?");
        });
    }

    function getFileAsBlob(elementId, query) {
        var pond = ponds[elementId];

        var file;

        if (!query)
            file = pond.getFile().file;
        else
            file = pond.getFile(query).file;

        const blob = new Blob([file]);
        return blob;
    }

    function getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => {
            if (typeof arg === 'object' && arg !== null) {
                return objectToStringifyable(arg);
            } else {
                return arg;
            }
        });

        var json = JSON.stringify(processedArgs);
        return json;
    }

    function getJsonFromObjectOrArray(objOrArray) {
        const stringifyable = mapToJSON(objOrArray);

        const json = JSON.stringify(stringifyable);
        return json;
    }

    function mapToJSON(objOrArray) {
        if (Array.isArray(objOrArray)) {
            return objOrArray.map(item => {
                return typeof item === 'object' && item !== null
                    ? objectToStringifyable(item)
                    : item;
            });
        } else if (typeof objOrArray === 'object' && objOrArray !== null) {
            return objectToStringifyable(objOrArray);
        } else {
            return objOrArray;
        }
    }

    function objectToStringifyable(obj) {
        let objectJSON = {};

        // Get all property names of the object
        let props = Object.getOwnPropertyNames(obj);

        // Iterate through each property
        props.forEach(prop => {
            // Get the property descriptor
            let descriptor = Object.getOwnPropertyDescriptor(obj, prop);

            // Check if the property has a getter
            if (descriptor && typeof descriptor.get === 'function') {
                // Call the getter in the context of the object
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                // Include the property value if there's no getter
                const propValue = obj[prop];

               if (typeof propValue === 'object' && propValue !== null) {
                    // Recursively handle nested objects
                    objectJSON[prop] = objectToStringifyable(propValue);
                } else {
                    objectJSON[prop] = propValue;
                }
            }
        });

        return objectJSON;
    }

    return {
        create: create,
        setOptions: setOptions,
        addFileFromStream: addFileFromStream,
        addFile: addFile,
        addFiles: addFiles,
        removeFile: removeFile,
        removeFiles: removeFiles,
        processFile: processFile,
        processFiles: processFiles,
        prepareFile: prepareFile,
        prepareFiles: prepareFiles,
        getFile: getFile,
        getFiles: getFiles,
        browse: browse,
        sort: sort,
        moveFile: moveFile,
        destroy: destroy,
        addOutputEventListener: addOutputEventListener,
        addEventListener: addEventListener,
        enablePlugins: enablePlugins,
        getFileAsBlob: getFileAsBlob
    };
})();