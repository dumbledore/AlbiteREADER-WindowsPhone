/*
 * I am not using prototypes for the emulation of a class method, because
 * the classes that use this are going to be available in very small counts,
 * thus speed and encapsulation are more important than small memory concerns.
 */
function Albite(mainWindow, pageWidth, initialLocation, debugEnabled) {
    /*
     * Attach the error handler for the main window
     */
    mainWindow.onerror = handleError;

    /*
     * Public API
     */

    /*
     * Called when an iframe has loaded
     */
    this.contentLoaded  = contentLoaded;

    /*
     * Go to a particular page.
     */
    this.goToPage       = goToPage;

    /*
     * Get the amount of available pages
     */
    this.getPageCount   = getPageCount;

    /*
     * Get the current page number
     */
    this.getCurrentPageNumber = getCurrentPageNumber;

    /*
     * Get the page for the specified DOM Location
     */
    this.getPageForLocation = getPageForLocation;

    /*
     * Pointer event handling
     */
    this.press      = press;
    this.move       = move;
    this.release    = release;

    /*
     * Functions for debugging
     */
    function log(msg, isError) {
        try {
            /*
             * Try reporting to .NET
             */
            var msgType = isError ? "{error}" : "{debug}";
            notifyServer(msgType + msg);
        } catch (e1) {
            try {
                /*
                 * Try the console (e.g. Chrome)
                 */
                console.log(msg);
            } catch (e2) {
                /*
                 * Last resort
                 */
                alert(msg);
            }
        }
    }

    function sleep(millis) {
        var date = new Date();
        var curDate = null;
        do {
            curDate = new Date();
        } while (curDate - date < millis);
    }

    function handleError(msg, url, line) {
        log(msg + " on line " + line + " for " + url, true);
    }

    function notifyServer(command) {
        window.external.notify(command);
    }

    function notifyLoaded() {
        notifyServer("{loaded}");
    }

    /*
     * Implementations
     */
    var currentPageNumber = 1;

    var PREVIOUS_PAGE_POSITION  = "0px";
    var CURRENT_PAGE_POSITION   = pageWidth + "px";
    var NEXT_PAGE_POSITION      = (pageWidth * 2) + "px";

    var CONTENT_CSS_FILE_NAME   = "/albite/content.css";

    /*
     * PageView is a mutable object that contains everything, necessary for
     * rendering a particular page.
     */
    function PageView(pageContent) {
        this.pageMetrics    = null;
        this.setPage        = setPage;
        this.setPosition    = setPosition;

        var contentFrame =
                pageContent.getElementsByClassName("page_content_frame")[0];

        var contentWindow = contentFrame.contentWindow;

        var contentClipper =
                pageContent.getElementsByClassName("page_content_clipper")[0];

        function setPage(pageMetrics) {
            this.pageMetrics    = pageMetrics;
            var pageNumber      = pageMetrics.pageNumber;

            /*
             * Hide the page while being set up
             */
            pageContent.style.visibility = "hidden";

            if (pageNumber <= 0 || pageNumber >= booklet.length - 1) {
                /*
                 * This is a first/last page dummy. Nothing more to do,
                 * just leave it hidden.
                 */
                return;
            }

            /*
             * Now update the displayed data
             */
            contentWindow.scrollTo(0, pageMetrics.offsetTop);
            contentClipper.style.height = pageMetrics.clipHeight + "px";

            /*
             * Now show it
             */
            pageContent.style.visibility = "visible";
        }

        function setPosition(pos) {
            pageContent.style.left = pos;
        }
    }

    /*
     * PageMetrics is an immutable object, the result of the
     * pagination process.
     */
    function PageMetrics(pageNumber, offsetTop, offsetBottom, startNode) {
        this.pageNumber = pageNumber;
        this.offsetTop  = offsetTop;
        this.clipHeight = offsetBottom - offsetTop + 1;
        this.startNode  = startNode;
    }

    // Private stuff
    var previousPage;
    var currentPage;
    var nextPage;

    /*
     * This will be an array of PageMetrics objects,
     * describing all objects in the chapter.
     */
    var booklet;

    /*
     * Initial loading
     */
    var numberOfFramesLoaded = 0;

    function contentLoaded(contentFrame) {
        /*
         * Attach the error handler
         */
        contentFrame.contentWindow.onerror = handleError;

        /*
         * This works because layout/render and javascript are synchronized
         * by running on the same thread.
         */
        numberOfFramesLoaded += 1;

        switch (numberOfFramesLoaded) {
            case 1:
                contentLoaded1(contentFrame);
                break;
            case 2:
                contentLoaded2(contentFrame);
                break;
            case 3:
                contentLoaded3(contentFrame);
                break;
            default:
                throw("Loaded more frames than necessary: " + numberOfFramesLoaded);
        }
    }

    function contentLoaded1(contentFrame) {
        var doc = contentFrame.contentWindow.document;
        var pageDiv = getParentByClassName(contentFrame, "page");

        previousPage = new PageView(pageDiv);

        loadCssFile(CONTENT_CSS_FILE_NAME, doc, function() {
            createFrame(pageDiv);
        });
    }

    function contentLoaded2(contentFrame) {
        var doc = contentFrame.contentWindow.document;
        var pageDiv = getParentByClassName(contentFrame, "page");

        currentPage = new PageView(pageDiv);

        loadCssFile(CONTENT_CSS_FILE_NAME, doc, function() {
            createFrame(pageDiv);
        });
    }

    function contentLoaded3(contentFrame) {
        var doc = contentFrame.contentWindow.document;
        var pageDiv = getParentByClassName(contentFrame, "page");

        nextPage = new PageView(pageDiv);

        loadCssFile(CONTENT_CSS_FILE_NAME, doc, function() {
            /*
             * This will create the bunch of page metrics, called a booklet.
             */
            booklet = paginate(
                contentFrame.scrollHeight,
                contentFrame.contentWindow.document.body);

            /*
             * Set the pages to be shown.
             */
            setupInitialLocation();

            /*
             * Ready to lift the curtains.
             */
            mainWindow.document.body.style.visibility = 'visible';

            /*
             * Notify the server that the page has loaded.
             */
            notifyLoaded();
        });
    }

    function setupInitialLocation() {
        var type = typeof initialLocation;
        switch (type) {
            case "number":
                goToPage(initialLocation);
                break;

            case "string":
                switch(initialLocation) {
                    case "first":
                        goToFirstPage();
                        break;

                    case "last":
                        goToLastPage();
                        break;

                    default:
                        throw("Invalid string for the initial location: " + initialLocation);
                }
                break;

            case "object":
                goToDomLocation(initialLocation.elementIndex, initialLocation.textOffset);
                break;

            default:
                throw("Invalid type for the initial location: " + type);
        }
    }

    function createFrame(pageDiv) {
        pageDiv.parentElement.appendChild(pageDiv.cloneNode(true));
    }

    function getParentByClassName(el, className) {
        var classNameMiddle = ' ' + className + ' ';

        var name;
        var index;

        for (; el; el = el.parentElement) {
            name = el.className;
            index = name.indexOf(className);

            if (index < 0) {
                continue;
            }

            if (className.length == name.length) {
                return el;
            }

            if (index == 0 && name[className.length] == ' ') {
                return el;
            }

            if (index == name.length - className.length) {
                return el;
            }

            if (name.indexOf(classNameMiddle) > 0) {
                return el;
            }
        }

        return null;
    }

    function loadCssFile(filename, doc, callback) {
        var linkElement = doc.createElement("link");

        linkElement.setAttribute("rel", "stylesheet");
        linkElement.setAttribute("type", "text/css");
        linkElement.setAttribute("href", filename);

        if (callback) {
            linkElement.addEventListener('load', callback);
        }

        doc.getElementsByTagName("head")[0].appendChild(linkElement);
    }

    /*
     * Location
     */
    function getPageForLocation(elementIndex, textOffset) {
        // TODO
        return 1;
    }

    function firstPageNumber() {
        return 1;
    }

    function lastPageNumber() {
        return booklet.length - 2;
    }

    function isFirstPage(pageNumber) {
        return pageNumber <= firstPageNumber();
    }

    function isLastPage(pageNumber) {
        return pageNumber >= lastPageNumber();
    }

    function goToFirstPage() {
        goToPage(firstPageNumber());
    }

    function goToLastPage() {
        goToPage(lastPageNumber());
    }

    function goToDomLocation(elementIndex, textOffset) {
        goToPage(getPageForLocation(elementIndex, textOffset));
    }

    function validatePageNumber(pageNumber) {
        if (typeof(pageNumber) != "number") {
            pageNumber = 1;
        }

        if (isFirstPage(pageNumber)) {
            pageNumber = firstPageNumber();
        }

        if (isLastPage(pageNumber)) {
            pageNumber = lastPageNumber();
        }

        return pageNumber;
    }

    function goToPage(pageNumber) {
        currentPageNumber = validatePageNumber(pageNumber);

        currentPage.setPage(booklet[pageNumber]);
        currentPage.setPosition(CURRENT_PAGE_POSITION);

        previousPage.setPage(booklet[currentPageNumber - 1]);
        nextPage.setPage(booklet[currentPageNumber + 1]);

        previousPage.setPosition(PREVIOUS_PAGE_POSITION);
        nextPage.setPosition(NEXT_PAGE_POSITION);

        resetWindow();
    }

    function getPageCount() {
        return booklet.length;
    }

    function getCurrentPageNumber() {
        return currentPageNumber;
    }

    /*
     * Scrolling and animation
     */
    function resetWindow() {
        window.scrollTo(pageWidth);
    }

    /*
     * Touch handling
     */
    var moved = false;

    function press(x, y) {
        moved = false;
        log("[press] at (" + x + ", " + y + ")");
    }

    function move(dx, dy) {
        moved = true;
        log("[move] at (" + dx + ", " + dy + ")");
    }

    function release(x, y, velocityX, velocityY) {
        log("[release] at (" + x + ", " + y + ") with velocity (" + velocityX + ", " + velocityY + ")");
    }

    /*
     * Pagination
     */
    function paginate(pageHeight, body) {

        var contentHeight = body.scrollHeight;

        function PageContent(element, rect) {
            this.element    = element;
            this.top        = rect.top;
            this.bottom     = rect.bottom;
        }

        function sortContent(a, b) {
            return b.top - a.top;
        }

        var content = [];

        function selectContent(node, content, range) {
            if (node.nodeType == 3) { // Hard code Node.TEXT_NODE for IE
                /*
                 * For text nodes use a Range to create a list of ClientRects.
                 * This is a very direct way of finding out the dimensions of
                 * the lines for all elements that contain text
                 * (like <p> and <span>, etc.) without any need to modify the
                 * document in any way.
                 */
                range.selectNodeContents(node);

                /*
                 * If the text in a Text node is not displayed at all,
                 * Range.getClientRects() will return an empty list and thus
                 * won't affect the list of PaginationPoints at all.
                 */
                var rects = range.getClientRects();
                var l = rects.length;
                for (var i = 0; i < l; i++) {
                    content.push(new PageContent(node, rects[i]));
                }
            } else if (node.tagName == "img") {
                /*
                 * As the image consists of only one part, directly use the
                 * bounding rectangle.
                 */
                content.push(new PageContent(node, node.getBoundingClientRect()));
            }

            node = node.firstChild;
            while (node) {
                selectContent(node, content, range);
                node = node.nextSibling;
            }
        }

        /*
         * Make a list of all content that needs to be paginated
         */
        selectContent(body, content, body.ownerDocument.createRange());

        /*
         * Sort the content rectangles in reverse, because we'll be popping,
         * which is done from the end of the list.
         *
         * sortContent() uses the top values for reference.
         */
        content.sort(sortContent);

        pages = [];

        /*
         * Add the first (left) dummy page
         */
        pages.push(new PageMetrics(0, 0, 0));

        /*
         * If for some reason there were no visible text/image elements,
         * simply create a single empty page and the right dummy page
         * and return.
         */
        if (content.length < 1) {
            /*
             * Empty page
             */
            pages.push(new PageMetrics(pages.length + 1, 0, 0));

            /*
             * Right dummy page
             */
            pages.push(new PageMetrics(pages.length + 1, 0, 0));
            return pages;
        }

        var ct          = content.pop();
        var pageStart   = ct;
        var pageBottom  = ct.top + pageHeight - 1;
        var page;

        while (ct) {
            /*
             * Does the content start on this page at all?
             */
            if (ct.top > pageBottom) {
                /*
                 * This piece of content starts after this page. As the list is
                 * sorted by the `top` values, all next pieces of content will
                 * start after this page as well. Create a new page.
                 */
                // log("Element " + ct.element + " doesn't start on this page. Ending page.");
                /*
                 * Create the page and add it to the list.
                 */
                page = new PageMetrics(
                    pages.length, pageStart.top,
                    pageBottom, pageStart.element);
                pages.push(page);

                /*
                 * Update the stuff for the next page.
                 */
                pageStart = ct;
                pageBottom = ct.top + pageHeight - 1;

                /*
                 * Don't pop the content. This one might need to be processed
                 * further, so simply continue.
                 */
                continue;
            }

            /*
             * This content starts on this page.
             */
            if (ct.bottom <= pageBottom) {
                /*
                 * It ends here as well. Continue with the next content.
                 */
                // log("Element " + ct.element + " ends on the current page. Continuing on.");
                ct = content.pop();
                continue;
            }

            /*
             * This content doesn't end on this page. A new page will
             * be created. But is it too tall so it would have to be clipped
             * in two or would it fit into the next page?
             */
            if (ct.bottom - ct.top < pageHeight && ct.element.tagName != "img") {
                /*
                 * The content would fit into the next page. No need to split.
                 * Simply create a new page.
                 */
                // log("Element " + ct.element + " doesn't end on this page. Moving to the next.");
                page = new PageMetrics(
                    pages.length, pageStart.top,
                    pageBottom < ct.top ? pageBottom : ct.top, pageStart.element);
                pages.push(page);

                /*
                 * Update the stuff for the next page.
                 */
                pageStart = ct;
                pageBottom = ct.top + pageHeight;

                /*
                 * This content won't need further processing as we just
                 * checked that. Continue with the next content;
                 */
                ct = content.pop();
                continue;
            }

            /*
             * This content would need to be clipped.
             */
            // log("Element " + ct.element + " needs to be clipped.");

            /*
             * Create a new PageContent, starting from the new top position,
             * add it to the list and then sort the list again, so that the
             * content would be on the next page and elements on this page
             * would still be processed
             */
            content.push(new PageContent(ct.element, { "top" : pageBottom + 1, "bottom" : ct.bottom }));
            content.sort(sortContent);
            ct = content.pop();
            continue;
        }

        /*
         * Don't forget to add the last page of the content!
         */
        page = new PageMetrics(
            pages.length, pageStart.top, pageBottom, pageStart.element);
        pages.push(page);

        /*
         * Add the last (right) dummy page
         */
        pages.push(new PageMetrics(pages.length + 1, 0, 0));

        return pages;
    }
}
