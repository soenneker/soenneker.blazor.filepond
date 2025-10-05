export class FilePondInterop {
    constructor() {
        this.ponds = {};
        this.options = {};
        this.deleted = {};
        this.observer = null;
    }

    async create(elementId, options) {
        let pond;

        const element = document.getElementById(elementId);

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

    async waitForPond(elementId) {
        return new Promise((resolve) => {
            const checkPond = () => {
                const pond = this.ponds[elementId];
                if (pond) {
                    resolve(pond);
                } else {
                    setTimeout(checkPond, 100); // Check again after 100 milliseconds
                }
            };

            checkPond();
        });
    }

    async addFile(elementId, uri, options) {
        var pond = await this.waitForPond(elementId);

        if (!options) {
            pond.addFile(uri);
        } else {
            pond.addFile(uri, options);
        }
    }
    
    async addFileFromStream(elementId, streamRef, options) {
        var pond = await this.waitForPond(elementId);

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

    async addLimboFile(elementId, filename, options) {
        var pond = await this.waitForPond(elementId);

        // Use MIME type from options or default to application/octet-stream
        const fileType = (options && options.mimeType) || 'application/octet-stream';
        
        // Create a file object with minimal content to avoid zero-length issues
        const file = new File([''], filename, { type: fileType });
        
        if (!options) {
            pond.addFile(file);
        } else {
            pond.addFile(file, options);
        }
    }

    async addFiles(elementId, uris, options) {
        var pond = await this.waitForPond(elementId);

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
        return this.getJsonFromObjectOrArray(files);
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

        // Clean up file size observers
        if (this.fileSizeObservers && this.fileSizeObservers[elementId]) {
            this.fileSizeObservers[elementId].disconnect();
            delete this.fileSizeObservers[elementId];
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

    async enableOtherPlugins(plugins) {
        plugins.forEach(plugin => {
            const pluginVariable = window[plugin];
            if (pluginVariable) {
                FilePond.registerPlugin(pluginVariable);
            } else {
                console.error(`Could not load FilePond plugin (${pluginName}), are you sure the necessary script is on the page?`);
            }
        });
    }

    async getFileAsBlob(elementId, query) {
        const pond = this.ponds[elementId];
        const fileItem = query ? pond.getFile(query) : pond.getFile();
        
        if (!fileItem) {
            return new Blob([]);
        }
        
        const originalFile = fileItem.file;
        console.log(`Original file size for ${query}:`, originalFile ? originalFile.size : 'No file');
        
        // Try to get the prepared (transformed) file data first - this is now the default behavior
        try {
            const preparedFile = await pond.prepareFile(query);
            console.log(`Prepared file result for ${query}:`, preparedFile);
            
            if (preparedFile && preparedFile.output) {
                console.log(`Prepared file output type for ${query}:`, typeof preparedFile.output);
                console.log(`Prepared file output size for ${query}:`, preparedFile.output.size || 'No size property');
                
                // The prepared file output contains the transformed data
                if (typeof preparedFile.output === 'string') {
                    // Base64 encoded data
                    const byteCharacters = atob(preparedFile.output.split(',')[1] || preparedFile.output);
                    const byteNumbers = new Array(byteCharacters.length);
                    for (let i = 0; i < byteCharacters.length; i++) {
                        byteNumbers[i] = byteCharacters.charCodeAt(i);
                    }
                    const byteArray = new Uint8Array(byteNumbers);
                    const blob = new Blob([byteArray]);
                    console.log(`Created blob from base64 for ${query}, size:`, blob.size);
                    return blob;
                } else if (preparedFile.output instanceof Blob) {
                    // Already a blob
                    console.log(`Using prepared blob for ${query}, size:`, preparedFile.output.size);
                    return preparedFile.output;
                } else if (preparedFile.output instanceof ArrayBuffer) {
                    // ArrayBuffer
                    const blob = new Blob([preparedFile.output]);
                    console.log(`Created blob from ArrayBuffer for ${query}, size:`, blob.size);
                    return blob;
                }
            } else {
                console.log(`No prepared file output for ${query}, using original file`);
            }
        } catch (error) {
            console.warn(`Failed to get prepared file for ${query}, falling back to original file:`, error);
        }
        
        // Fallback to original file
        const file = fileItem.file;
        const fallbackBlob = file ? new Blob([file]) : new Blob([]);
        console.log(`Using fallback blob for ${query}, size:`, fallbackBlob.size);
        return fallbackBlob;
    }

    // New method to explicitly get the original (untransformed) file
    getOriginalFileAsBlob(elementId, query) {
        const pond = this.ponds[elementId];
        const fileItem = query ? pond.getFile(query) : pond.getFile();
        
        if (!fileItem) {
            return new Blob([]);
        }
        
        // Always return the original file
        const file = fileItem.file;
        return file ? new Blob([file]) : new Blob([]);
    }


    hasFileContent(elementId, query) {
        const pond = this.ponds[elementId];
        const file = query ? pond.getFile(query).file : pond.getFile().file;
        return file && file.size > 0;
    }

    // New method to get multiple files as blobs efficiently
    // This method processes files sequentially to avoid concurrency issues
    async getFilesAsBlobsSequential(elementId, fileIds) {
        if (!fileIds || fileIds.length === 0) {
            return [];
        }
        
        // Process files one by one using the existing getFileAsBlob method
        // This avoids concurrency issues while still being more efficient than multiple interop calls
        const results = [];
        for (const fileId of fileIds) {
            try {
                const blob = await this.getFileAsBlob(elementId, fileId);
                
                // Log transformation info
                console.log(`File ${fileId}: Original size vs Transformed size:`, blob.size);
                
                // Convert blob to array buffer to avoid serialization issues
                const arrayBuffer = await blob.arrayBuffer();
                results.push({
                    fileId: fileId,
                    data: new Uint8Array(arrayBuffer)
                });
            } catch (error) {
                console.error(`Error processing file ${fileId}:`, error);
                results.push({
                    fileId: fileId,
                    data: new Uint8Array(0)
                });
            }
        }
        
        return results;
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
        const target = document.getElementById(elementId);
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

    setFileSizeVisibility(elementId, showFileSize = true) {
        // Find the FilePond element by ID
        const element = document.getElementById(elementId);
        
        if (!element) {
            console.warn(`Could not find FilePond element with ID: ${elementId}`);
            return;
        }

        // Find the FilePond root element
        let pondRoot = element.closest('.filepond--root');
        
        if (!pondRoot) {
            // If the element itself is the root
            if (element.classList.contains('filepond--root')) {
                pondRoot = element;
            } else {
                console.warn(`Could not find FilePond root for element: ${elementId}`);
                return;
            }
        }

        // Function to apply file size visibility
        const applyFileSizeVisibility = () => {
            const fileInfoSubs = pondRoot.querySelectorAll('.filepond--file-info-sub');
            fileInfoSubs.forEach(infoSub => {
                infoSub.style.display = showFileSize ? '' : 'none';
            });
        };

        // Apply immediately if elements exist
        applyFileSizeVisibility();

        // Set up a mutation observer to watch for new file elements being added
        const observer = new MutationObserver((mutations) => {
            mutations.forEach((mutation) => {
                if (mutation.type === 'childList') {
                    mutation.addedNodes.forEach((node) => {
                        if (node.nodeType === Node.ELEMENT_NODE) {
                            // Check if the added node or its children contain file info elements
                            const fileInfoSubs = node.querySelectorAll ? node.querySelectorAll('.filepond--file-info-sub') : [];
                            if (fileInfoSubs.length > 0 || node.classList?.contains('filepond--file-info-sub')) {
                                applyFileSizeVisibility();
                            }
                        }
                    });
                }
            });
        });

        // Start observing the pond root for changes
        observer.observe(pondRoot, {
            childList: true,
            subtree: true
        });

        // Store the observer so we can disconnect it later if needed
        if (!this.fileSizeObservers) {
            this.fileSizeObservers = {};
        }
        this.fileSizeObservers[elementId] = observer;
    }

    setValidationState(elementId, isValid, errorMessage = null) {
        const wrapper = document.getElementById(elementId.replace('filepond-', 'filepond-wrapper-'));
        if (!wrapper) return;

        const pondRoot = wrapper.querySelector('.filepond--root');
        if (!pondRoot) return;

        // Remove existing error message
        wrapper.querySelector('.filepond-validation-error')?.remove();

        // Update validation state
        const invalidClass = 'filepond-invalid';
        const pondInvalidClass = 'filepond--invalid';
        
        if (isValid) {
            wrapper.classList.remove(invalidClass);
            pondRoot.classList.remove(pondInvalidClass);
        } else {
            wrapper.classList.add(invalidClass);
            pondRoot.classList.add(pondInvalidClass);
            
            // Add error message if provided
            if (errorMessage) {
                const errorDiv = document.createElement('div');
                errorDiv.className = 'filepond-validation-error';
                errorDiv.textContent = errorMessage;
                wrapper.appendChild(errorDiv);
            }
        }
    }

    setFileSuccess(elementId, query, isSuccess = true) {
        const pond = this.ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const file = pond.getFile(query);
        if (!file) {
            console.warn(`Could not find file with query: ${query}`);
            return;
        }

        this._setFileSuccessInternal(file, isSuccess, false);
    }

    setFileSuccessWhenReady(elementId, query, isSuccess = true) {
        const pond = this.ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const file = pond.getFile(query);
        if (!file) {
            console.warn(`Could not find file with query: ${query}`);
            return;
        }

        this._setFileSuccessInternal(file, isSuccess, true);
    }

    setAllFilesSuccess(elementId, isSuccess = true) {
        const pond = this.ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const files = pond.getFiles();
        this._setFilesSuccessInternal(files, isSuccess, false);
    }

    setAllFilesSuccessWhenReady(elementId, isSuccess = true) {
        const pond = this.ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const files = pond.getFiles();
        this._setFilesSuccessInternal(files, isSuccess, true);
    }

    // Helper method to set success state for a single file
    _setFileSuccessInternal(file, isSuccess, waitForReady = false) {
        // Set metadata first
        if (isSuccess) {
            file.setMetadata('success', true);
        } else {
            file.setMetadata('success', false);
        }

        // Apply styling function
        const applyStyling = () => {
            // Check if we need to wait for the file to be ready
            // File is ready when it's either idle (1), processing queued (2), processing complete (4), or has been processed
            if (waitForReady && file.status !== 1 && file.status !== 2 && file.status !== 4) { // 1 = Idle, 2 = ProcessingQueued, 4 = ProcessingComplete
                return false;
            }

            // Find the file element by searching for the filepond--item with the correct ID
            // Try multiple selectors to find the file element
            let fileElement = document.querySelector(`[id*="${file.id}"]`);
            if (!fileElement || !fileElement.classList.contains('filepond--item')) {
                // Try alternative selectors
                fileElement = document.querySelector(`[data-filepond-item-id="${file.id}"]`);
            }
            if (!fileElement || !fileElement.classList.contains('filepond--item')) {
                // Try finding by filepond--item class and check if it contains the file ID
                const items = document.querySelectorAll('.filepond--item');
                for (const item of items) {
                    if (item.id && item.id.includes(file.id)) {
                        fileElement = item;
                        break;
                    }
                }
            }
            if (fileElement && fileElement.classList.contains('filepond--item')) {
                if (isSuccess) {
                    fileElement.classList.add('filepond--item-success');
                    // Also set the data attribute to ensure FilePond's default styling is overridden
                    fileElement.setAttribute('data-filepond-item-state', 'processing-complete');
                } else {
                    fileElement.classList.remove('filepond--item-success');
                    // Remove the data attribute to revert to default state
                    fileElement.removeAttribute('data-filepond-item-state');
                }
                return true;
            }
            return false;
        };

        // Try immediately
        if (!applyStyling()) {
            if (waitForReady) {
                // Listen for the file to be processed
                const checkInterval = setInterval(() => {
                    if (applyStyling()) {
                        clearInterval(checkInterval);
                    }
                }, 100);

                // Stop checking after 10 seconds to prevent infinite loops
                setTimeout(() => {
                    clearInterval(checkInterval);
                    // Try to apply styling one more time even if file isn't ready
                    applyStyling();
                }, 10000);
            } else {
                // Retry mechanism for immediate styling
                setTimeout(() => {
                    if (!applyStyling()) {
                        setTimeout(applyStyling, 500);
                    }
                }, 100);
            }
        }
    }

    // Helper method to set success state for multiple files
    _setFilesSuccessInternal(files, isSuccess, waitForReady = false) {
        // Set metadata for all files first
        files.forEach(file => {
            if (isSuccess) {
                file.setMetadata('success', true);
            } else {
                file.setMetadata('success', false);
            }
        });

        // Apply styling function
        const applyStyling = () => {
            let allReady = true;
            let allStyled = true;
            
            files.forEach(file => {
                // Check if we need to wait for the file to be ready
                // File is ready when it's either idle (1), processing queued (2), processing complete (4), or has been processed
                if (waitForReady && file.status !== 1 && file.status !== 2 && file.status !== 4) { // 1 = Idle, 2 = ProcessingQueued, 4 = ProcessingComplete
                    allReady = false;
                    return;
                }
                
                // Find the file element by searching for the filepond--item with the correct ID
                // Try multiple selectors to find the file element
                let fileElement = document.querySelector(`[id*="${file.id}"]`);
                if (!fileElement || !fileElement.classList.contains('filepond--item')) {
                    // Try alternative selectors
                    fileElement = document.querySelector(`[data-filepond-item-id="${file.id}"]`);
                }
                if (!fileElement || !fileElement.classList.contains('filepond--item')) {
                    // Try finding by filepond--item class and check if it contains the file ID
                    const items = document.querySelectorAll('.filepond--item');
                    for (const item of items) {
                        if (item.id && item.id.includes(file.id)) {
                            fileElement = item;
                            break;
                        }
                    }
                }
                if (fileElement && fileElement.classList.contains('filepond--item')) {
                    if (isSuccess) {
                        fileElement.classList.add('filepond--item-success');
                        // Also set the data attribute to ensure FilePond's default styling is overridden
                        fileElement.setAttribute('data-filepond-item-state', 'processing-complete');
                    } else {
                        fileElement.classList.remove('filepond--item-success');
                        // Remove the data attribute to revert to default state
                        fileElement.removeAttribute('data-filepond-item-state');
                    }
                } else {
                    allStyled = false;
                }
            });
            
            return waitForReady ? (allReady && allStyled) : allStyled;
        };

        // Try immediately
        if (!applyStyling()) {
            if (waitForReady) {
                // Listen for all files to be processed
                const checkInterval = setInterval(() => {
                    if (applyStyling()) {
                        clearInterval(checkInterval);
                    }
                }, 100);

                // Stop checking after 10 seconds to prevent infinite loops
                setTimeout(() => {
                    clearInterval(checkInterval);
                    // Try to apply styling one more time even if files aren't ready
                    applyStyling();
                }, 10000);
            } else {
                // Retry mechanism for immediate styling
                setTimeout(() => {
                    if (!applyStyling()) {
                        setTimeout(applyStyling, 500);
                    }
                }, 100);
            }
        }
    }

}

window.FilePondInterop = new FilePondInterop();