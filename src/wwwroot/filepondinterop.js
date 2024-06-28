import '../Soenneker.Blazor.Utils.ResourceLoader/resourceloader.js';

export class FilePondInterop {
    constructor() {
        this.ponds = {};
        this.options = {};
        this.deleted = {};
        this.observer = null;
    }

    async loadScriptAndWait() {
        await ResourceLoader.loadScript('https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.js', "sha256-6yXpr8+sATA4Q2ANTyZmpn4ZGP7grbIRNpe9s0Y+iO0=");
        await ResourceLoader.waitForVariable("FilePond");
    }

    async create(elementId, options) {
        await ResourceLoader.loadCss('https://cdn.jsdelivr.net/npm/filepond@4.31.1/dist/filepond.min.css', "sha256-a95jYCBL4++k1XyLYgulKmY33bIJIVYMsJO/RNytaJM=");
        await this.loadScriptAndWait();

        let pond;

        const selector = `[blazor-interop-id="${elementId}"]`;
        const element = document.querySelector(selector);

        if (options) {
            const opt = JSON.parse(options);
            pond = FilePond.create(element, opt);
            this.options[elementId] = opt;
        } else {
            pond = FilePond.create(element);
        }

        this.ponds[elementId] = pond;
    }

    setOptions(elementId, options) {
        const pond = this.ponds[elementId];
        const opt = JSON.parse(options);
        this.options[elementId] = opt;
        pond.setOptions(opt);
    }

    addFile(elementId, uri, options) {
        const pond = this.ponds[elementId];
        if (!options) {
            pond.addFile(uri);
        } else {
            pond.addFile(uri, options);
        }
    }

    async addFileFromStream(elementId, streamRef, options) {
        const pond = this.ponds[elementId];
        const readableStream = await streamRef.stream();
        const response = new Response(readableStream);

        try {
            response.blob().then((blob) => {
                if (!options) {
                    pond.addFile(blob);
                } else {
                    pond.addFile(blob, options);
                }
            });
        } catch (error) {
            console.error('Error adding file from stream:', error);
        }
    }

    addFiles(elementId, uris, options) {
        const pond = this.ponds[elementId];
        if (!options) {
            pond.addFiles(uris);
        } else {
            pond.addFiles(uris, options);
        }
    }

    removeFile(elementId, query) {
        const pond = this.ponds[elementId];
        if (!query) {
            pond.removeFile();
        } else {
            pond.removeFile(query);
        }
    }

    removeFiles(elementId, query) {
        const pond = this.ponds[elementId];
        if (!query) {
            pond.removeFiles();
        } else {
            pond.removeFiles(query);
        }
    }

    processFile(elementId, query) {
        const pond = this.ponds[elementId];
        if (!query) {
            pond.processFile();
        } else {
            pond.processFile(query);
        }
    }

    processFiles(elementId, query) {
        const pond = this.ponds[elementId];
        if (!query) {
            pond.processFiles();
        } else {
            pond.processFiles(query);
        }
    }

    prepareFile(elementId, query) {
        const pond = this.ponds[elementId];
        return pond.prepareFile(query);
    }

    prepareFiles(elementId, query) {
        const pond = this.ponds[elementId];
        return pond.prepareFiles(query);
    }

    getFile(elementId, query) {
        const pond = this.ponds[elementId];
        const file = pond.getFile(query);
        return this.getJsonFromObjectOrArray(file);
    }

    getFiles(elementId) {
        const pond = this.ponds[elementId];
        const files = pond.getFiles();
        return this.getJsonFromObject(files);
    }

    browse(elementId) {
        const pond = this.ponds[elementId];
        pond.browse();
    }

    sort(elementId, compare) {
        const pond = this.ponds[elementId];
        const compareFn = window[compare];
        pond.getFiles().sort(compareFn);
    }

    moveFile(elementId, query, index) {
        const pond = this.ponds[elementId];
        pond.moveFile(query, index);
    }

    destroy(elementId) {
        const pond = this.ponds[elementId];
        const hasBeenDeleted = this.deleted[elementId];

        if (pond && !hasBeenDeleted) {
            this.deleted[elementId] = true;
            pond.destroy();
            delete this.ponds[elementId];
            delete this.options[elementId];
        }
    }

    addEventListener(elementId, eventName, dotNetCallback) {
        const pond = this.ponds[elementId];
        pond.on(eventName, (...args) => {
            const json = this.getJsonFromArguments(...args);
            return dotNetCallback.invokeMethodAsync("Invoke", json);
        });
    }

    addOutputEventListener(elementId, eventName, dotNetCallback) {
        const pond = this.ponds[elementId];

        const handleFileEvent = (file) => {
            const filePondItemJson = this.getJsonFromObjectOrArray(file);
            return dotNetCallback.invokeMethodAsync("InvokeWithOutput", filePondItemJson).then((data) => data);
        };

        const handleRenameEvent = (file) => {
            const json = JSON.stringify(file);
            return dotNetCallback.invokeMethodAsync("InvokeWithOutput", json).then((data) => data);
        };

        switch (eventName) {
            case "OnBeforeAddFile":
                pond.beforeAddFile = handleFileEvent;
                break;
            case "OnBeforeDropFile":
                pond.beforeDropFile = handleFileEvent;
                break;
            case "OnBeforeRemoveFile":
                pond.beforeRemoveFile = handleFileEvent;
                break;
            case "OnFileRename":
                let existingOptions = this.options[elementId];
                if (!existingOptions) existingOptions = {};
                existingOptions.fileRenameFunction = handleRenameEvent;
                pond.setOptions(existingOptions);
                this.options[elementId] = existingOptions;
                break;
        }
    }

    async enablePlugins(plugins) {
        await this.loadScriptAndWait();

        plugins.forEach(plugin => {
            const pluginName = `FilePondPlugin${plugin}`;
            const pluginVariable = window[pluginName];
            if (pluginVariable) {
                FilePond.registerPlugin(pluginVariable);
            } else {
                console.error(`Could not load FilePond plugin (${pluginName}), are you sure the necessary script is on the page?`);
            }
        });
    }

    getFileAsBlob(elementId, query) {
        const pond = this.ponds[elementId];
        const file = query ? pond.getFile(query).file : pond.getFile().file;
        return new Blob([file]);
    }

    getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => (typeof arg === 'object' && arg !== null) ? this.objectToStringifyable(arg) : arg);
        return JSON.stringify(processedArgs);
    }

    getJsonFromObjectOrArray(objOrArray) {
        const stringifyable = this.mapToJSON(objOrArray);
        return JSON.stringify(stringifyable);
    }

    mapToJSON(objOrArray) {
        if (Array.isArray(objOrArray)) {
            return objOrArray.map(item => (typeof item === 'object' && item !== null) ? this.objectToStringifyable(item) : item);
        } else if (typeof objOrArray === 'object' && objOrArray !== null) {
            return this.objectToStringifyable(objOrArray);
        } else {
            return objOrArray;
        }
    }

    objectToStringifyable(obj) {
        const objectJSON = {};
        const props = Object.getOwnPropertyNames(obj);

        props.forEach(prop => {
            const descriptor = Object.getOwnPropertyDescriptor(obj, prop);
            if (descriptor && typeof descriptor.get === 'function') {
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                const propValue = obj[prop];
                objectJSON[prop] = (typeof propValue === 'object' && propValue !== null) ? this.objectToStringifyable(propValue) : propValue;
            }
        });

        return objectJSON;
    }

    createObserver(elementId) {
        const target = document.querySelector(`div[blazor-observer-id="${elementId}"]`);
        this.observer = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation => Array.from(mutation.removedNodes).indexOf(target) !== -1);

            if (targetRemoved) {
                this.destroy(elementId);

                this.observer && this.observer.disconnect();
                delete this.observer;
            }
        });

        this.observer.observe(target.parentNode, { childList: true });
    }
}

window.FilePondInterop = new FilePondInterop();