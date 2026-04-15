const ponds = {};
const fpOptions = {};
const deletedFlags = {};
const serverProcesses = {};
let pondMutationObserver = null;
let fileSizeObservers;

export async function create(elementId, options, dotNetCallback, useBlazorServerProcess = false) {
        let pond;

        const element = document.getElementById(elementId);

        if (options) {
            const opt = JSON.parse(options);
            if (useBlazorServerProcess) {
                opt.server = opt.server || {};
                opt.server.process = createBlazorServerProcessHandler(elementId, dotNetCallback);
            }
            pond = FilePond.create(element, opt);
            fpOptions[elementId] = opt;
        } else {
            const opt = useBlazorServerProcess ? { server: { process: createBlazorServerProcessHandler(elementId, dotNetCallback) } } : undefined;
            pond = FilePond.create(element, opt);
            if (opt) {
                fpOptions[elementId] = opt;
            }
        }

        ponds[elementId] = pond;
}
export function setOptions(elementId, options, dotNetCallback, useBlazorServerProcess = false) {
        const pond = ponds[elementId];
        const opt = JSON.parse(options);
        if (useBlazorServerProcess) {
            opt.server = opt.server || {};
            opt.server.process = createBlazorServerProcessHandler(elementId, dotNetCallback);
        }
        fpOptions[elementId] = opt;
        pond.setOptions(opt);
}
export async function waitForPond(elementId) {
        return new Promise((resolve) => {
            const checkPond = () => {
                const pond = ponds[elementId];
                if (pond) {
                    resolve(pond);
                } else {
                    setTimeout(checkPond, 100); // Check again after 100 milliseconds
                }
            };

            checkPond();
        });
}
export async function addFile(elementId, uri, options) {
        var pond = await waitForPond(elementId);

        if (!options) {
            pond.addFile(uri);
        } else {
            pond.addFile(uri, options);
        }
}
export async function addFileFromStream(elementId, streamRef, options) {
        var pond = await waitForPond(elementId);

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
export async function addLimboFile(elementId, filename, options) {
        var pond = await waitForPond(elementId);

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
export async function addFiles(elementId, uris, options) {
        var pond = await waitForPond(elementId);

        if (!options) {
            pond.addFiles(uris);
        } else {
            pond.addFiles(uris, options);
        }
}
export function removeFile(elementId, query) {
        const pond = ponds[elementId];
        if (!query) {
            pond.removeFile();
        } else {
            pond.removeFile(query);
        }
}
export function removeFiles(elementId, query) {
        const pond = ponds[elementId];
        if (!query) {
            pond.removeFiles();
        } else {
            pond.removeFiles(query);
        }
}
export function processFile(elementId, query) {
        const pond = ponds[elementId];
        if (!query) {
            pond.processFile();
        } else {
            pond.processFile(query);
        }
}
export function processFiles(elementId, query) {
        const pond = ponds[elementId];
        if (!query) {
            pond.processFiles();
        } else {
            pond.processFiles(query);
        }
}
export function prepareFile(elementId, query) {
        const pond = ponds[elementId];
        return pond.prepareFile(query);
}
export function prepareFiles(elementId, query) {
        const pond = ponds[elementId];
        return pond.prepareFiles(query);
}
export function getFile(elementId, query) {
        const pond = ponds[elementId];
        const file = pond.getFile(query);
        return getJsonFromObjectOrArray(file);
}
export function getFiles(elementId) {
        const pond = ponds[elementId];
        const files = pond.getFiles();
        return getJsonFromObjectOrArray(files);
}
export function browse(elementId) {
        const pond = ponds[elementId];
        pond.browse();
}
export function sort(elementId, compare) {
        const pond = ponds[elementId];
        const compareFn = window[compare];
        pond.getFiles().sort(compareFn);
}
export function moveFile(elementId, query, index) {
        const pond = ponds[elementId];
        pond.moveFile(query, index);
}
export function destroy(elementId) {
        const pond = ponds[elementId];
        const hasBeenDeleted = deletedFlags[elementId];

        if (pond && !hasBeenDeleted) {
            deletedFlags[elementId] = true;
            pond.destroy();
            delete ponds[elementId];
            delete fpOptions[elementId];
        }

        // Clean up file size observers
        if (fileSizeObservers && fileSizeObservers[elementId]) {
            fileSizeObservers[elementId].disconnect();
            delete fileSizeObservers[elementId];
        }

        Object.keys(serverProcesses)
            .filter(processId => serverProcesses[processId]?.elementId === elementId)
            .forEach(processId => delete serverProcesses[processId]);
}
export function createBlazorServerProcessHandler(elementId, dotNetCallback) {
        return (fieldName, file, metadata, load, error, progress, abort) => {
            const processId = createProcessId();
            const fileItem = findFileItemForProcess(elementId, file);

            if (!fileItem || !fileItem.id) {
                error('Could not resolve the FilePond file item for Blazor server.process');
                return {
                    abort: () => { }
                };
            }

            serverProcesses[processId] = {
                elementId,
                progress,
                load,
                error,
                abort,
                fileSize: file?.size ?? 0
            };

            const metadataJson = metadata == null ? null : JSON.stringify(metadata);

            dotNetCallback.invokeMethodAsync("ProcessFileJs", elementId, processId, fieldName, getJsonFromObjectOrArray(fileItem), metadataJson)
                .then((serverId) => {
                    const serverProcess = serverProcesses[processId];
                    if (!serverProcess) {
                        return;
                    }

                    if (serverProcess.fileSize > 0) {
                        serverProcess.progress(true, serverProcess.fileSize, serverProcess.fileSize);
                    }

                    serverProcess.load(serverId ?? '');
                    delete serverProcesses[processId];
                })
                .catch((processError) => {
                    const serverProcess = serverProcesses[processId];
                    if (!serverProcess) {
                        return;
                    }

                    serverProcess.error(getErrorMessage(processError));
                    delete serverProcesses[processId];
                });

            return {
                abort: () => {
                    const serverProcess = serverProcesses[processId];
                    if (!serverProcess) {
                        return;
                    }

                    dotNetCallback.invokeMethodAsync("AbortServerProcessJs", elementId, processId)
                        .catch((abortError) => console.warn('Error aborting Blazor server.process', abortError));

                    serverProcess.abort();
                    delete serverProcesses[processId];
                }
            };
        };
}
export function reportServerProcessProgress(elementId, processId, isLengthComputable, loaded, total) {
        const serverProcess = serverProcesses[processId];

        if (!serverProcess || serverProcess.elementId !== elementId) {
            return;
        }

        serverProcess.progress(isLengthComputable, loaded, total);
}
export function findFileItemForProcess(elementId, file) {
        const pond = ponds[elementId];

        if (!pond) {
            return null;
        }

        const files = pond.getFiles();
        let fileItem = files.find(item => item.file === file);

        if (fileItem) {
            return fileItem;
        }

        return files.find(item =>
            item.filename === file?.name &&
            item.fileSize === (file?.size ?? 0));
}
export function createProcessId() {
        if (window.crypto?.randomUUID) {
            return window.crypto.randomUUID();
        }

        return `${Date.now()}-${Math.random().toString(36).slice(2, 11)}`;
}
export function getErrorMessage(error) {
        if (!error) {
            return 'Upload failed';
        }

        if (typeof error === 'string') {
            return error;
        }

        return error.message || error.toString() || 'Upload failed';
}
export function addEventListener(elementId, eventName, dotNetCallback) {
        const pond = ponds[elementId];
        pond.on(eventName, (...args) => {
            const json = getJsonFromArguments(...args);
            return dotNetCallback.invokeMethodAsync("Invoke", json);
        });
}
export function addOutputEventListener(elementId, eventName, dotNetCallback) {
        const pond = ponds[elementId];

        const handleFileEvent = (file) => {
            const filePondItemJson = getJsonFromObjectOrArray(file);
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
                let existingOptions = fpOptions[elementId];
                if (!existingOptions) existingOptions = {};
                existingOptions.fileRenameFunction = handleRenameEvent;
                pond.setOptions(existingOptions);
                fpOptions[elementId] = existingOptions;
                break;
        }
}
export async function enablePlugins(plugins) {
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
export async function enableOtherPlugins(plugins) {
        plugins.forEach(plugin => {
            const pluginVariable = window[plugin];
            if (pluginVariable) {
                FilePond.registerPlugin(pluginVariable);
            } else {
                console.error(`Could not load FilePond plugin (${plugin}), are you sure the necessary script is on the page?`);
            }
        });
}

export async function getFileAsBlob(elementId, query) {
        const pond = ponds[elementId];
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
export function getOriginalFileAsBlob(elementId, query) {
        const pond = ponds[elementId];
        const fileItem = query ? pond.getFile(query) : pond.getFile();
        
        if (!fileItem) {
            return new Blob([]);
        }
        
        // Always return the original file
        const file = fileItem.file;
        return file ? new Blob([file]) : new Blob([]);
}
export function hasFileContent(elementId, query) {
        const pond = ponds[elementId];
        const file = query ? pond.getFile(query).file : pond.getFile().file;
        return file && file.size > 0;
}
export async function getFilesAsBlobsSequential(elementId, fileIds) {
        if (!fileIds || fileIds.length === 0) {
            return [];
        }
        
        // Process files one by one using the existing getFileAsBlob method
        // This avoids concurrency issues while still being more efficient than multiple interop calls
        const results = [];
        for (const fileId of fileIds) {
            try {
                const blob = await getFileAsBlob(elementId, fileId);
                
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
export function getJsonFromArguments(...args) {
        const processedArgs = args.map(arg => (typeof arg === 'object' && arg !== null) ? objectToStringifyable(arg) : arg);
        return JSON.stringify(processedArgs);
}
export function getJsonFromObjectOrArray(objOrArray) {
        const stringifyable = mapToJSON(objOrArray);
        return JSON.stringify(stringifyable);
}
export function mapToJSON(objOrArray) {
        if (Array.isArray(objOrArray)) {
            return objOrArray.map(item => (typeof item === 'object' && item !== null) ? objectToStringifyable(item) : item);
        } else if (typeof objOrArray === 'object' && objOrArray !== null) {
            return objectToStringifyable(objOrArray);
        } else {
            return objOrArray;
        }
}
export function objectToStringifyable(obj) {
        const objectJSON = {};
        const props = Object.getOwnPropertyNames(obj);

        props.forEach(prop => {
            const descriptor = Object.getOwnPropertyDescriptor(obj, prop);
            if (descriptor && typeof descriptor.get === 'function') {
                objectJSON[prop] = descriptor.get.call(obj);
            } else {
                const propValue = obj[prop];
                objectJSON[prop] = (typeof propValue === 'object' && propValue !== null) ? objectToStringifyable(propValue) : propValue;
            }
        });

        return objectJSON;
}
export function createObserver(elementId) {
        const target = document.getElementById(elementId);
        pondMutationObserver = new MutationObserver((mutations) => {
            const targetRemoved = mutations.some(mutation => Array.from(mutation.removedNodes).indexOf(target) !== -1);

            if (targetRemoved) {
                destroy(elementId);

                pondMutationObserver && pondMutationObserver.disconnect();
                pondMutationObserver = null;
            }
        });

        pondMutationObserver.observe(target.parentNode, { childList: true });
}
export function setFileSizeVisibility(elementId, showFileSize = true) {
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
        if (!fileSizeObservers) {
            fileSizeObservers = {};
        }
        fileSizeObservers[elementId] = observer;
}
export function setValidationState(elementId, isValid, errorMessage = null) {
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
export function setFileSuccess(elementId, query, isSuccess = true) {
        const pond = ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const file = pond.getFile(query);
        if (!file) {
            console.warn(`Could not find file with query: ${query}`);
            return;
        }

        _setFileSuccessInternal(file, isSuccess, false);
}
export function setFileSuccessWhenReady(elementId, query, isSuccess = true) {
        const pond = ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const file = pond.getFile(query);
        if (!file) {
            console.warn(`Could not find file with query: ${query}`);
            return;
        }

        _setFileSuccessInternal(file, isSuccess, true);
}
export function setAllFilesSuccess(elementId, isSuccess = true) {
        const pond = ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const files = pond.getFiles();
        _setFilesSuccessInternal(files, isSuccess, false);
}
export function setAllFilesSuccessWhenReady(elementId, isSuccess = true) {
        const pond = ponds[elementId];
        if (!pond) {
            console.warn(`Could not find FilePond instance for element: ${elementId}`);
            return;
        }

        const files = pond.getFiles();
        _setFilesSuccessInternal(files, isSuccess, true);
}
export function _setFileSuccessInternal(file, isSuccess, waitForReady = false) {
        // Set metadata first
        if (isSuccess) {
            file.setMetadata('success', true);
        } else {
            file.setMetadata('success', false);
        }

        // Apply styling function
        const applyStyling = () => {
            // Check if we need to wait for the file to be ready
            // File is ready when it's either idle (2), processing queued (9), processing complete (5), or has been processed
            if (waitForReady && file.status !== 2 && file.status !== 9 && file.status !== 5) { // 2 = Idle, 9 = ProcessingQueued, 5 = ProcessingComplete
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
export function _setFilesSuccessInternal(files, isSuccess, waitForReady = false) {
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
                // File is ready when it's either idle (2), processing queued (9), processing complete (5), or has been processed
                if (waitForReady && file.status !== 2 && file.status !== 9 && file.status !== 5) { // 2 = Idle, 9 = ProcessingQueued, 5 = ProcessingComplete
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