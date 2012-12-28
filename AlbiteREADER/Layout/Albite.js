/*
 * I am not using prototypes for the emulation of a class method, because
 * the classes that use this are going to be available in very small counts,
 * thus speed and encapsulation are more important than small memory concerns.
 */
function Albite(mainWindow, pageWidth, currentPageNumber) {

    /*
     * Public API
     */
    this.contentLoaded  = contentLoaded;
    this.goToPage       = goToPage;

    /*
     * Functions for debugging
     */
    function debug(msg) {
        try {
            console.log(msg);
        } catch (e) {
            alert(msg);
        }
    }

    var startTime;

    function tic() {
        startTime = (new Date()).getMilliseconds();
    }

    function toc() {
        return (new Date()).getMilliseconds() - startTime;
    }

    /*
     * Implementations
     */

    var PREVIOUS_PAGE_POSITION  = "0px";
    var CURRENT_PAGE_POSITION   = pageWidth + "px";
    var NEXT_PAGE_POSITION      = (pageWidth * 2) + "px";

    /*
     * PageView is a mutable object that contains everything, necessary for
     * rendering a particular page.
     */
    function PageView(pageContent) {
        this.pageMetrics    = null;
        this.setPage        = setPage;
        this.setPosition    = setPosition;

        var header = pageContent.getElementsByClassName("page_header")[0];

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
             * Hide the header for the first page.
             */
            header.style.visibility = pageNumber != 1 ? "visible" : "hidden";

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
        this.pageNumber = pageNumber
        this.offsetTop  = offsetTop;
        this.clipHeight = offsetBottom - offsetTop;
        this.startNode  = startNode;
    }

    function ChapterPosition(textElement, textOffset) {
        this.textElement = textElement;
        this.textOffset = textOffset;
    }

    // Private stuff
    var numberOfFramesLoaded = 0;

    var previousPage;
    var currentPage;
    var nextPage;

    /*
     * This will be an array of PageMetrics objects,
     * describing all objects in the chapter.
     */
    var booklet;

    function contentLoaded(contentFrame) {
        numberOfFramesLoaded++;

        if (numberOfFramesLoaded == 1) {
            /*
             * First paginate. This will create the bunch of page metrics,
             * called a booklet.
             */
            booklet = paginate(
                contentFrame.scrollHeight,
                contentFrame.contentWindow.document.body);

            /*
             * Now create the page views
             */
            createPagesViews();
        }

        if (numberOfFramesLoaded == 3) {
            /*
             * Set the pages to be shown.
             */
            this.goToPage(currentPageNumber);

            /*
             * Ready to lift the curtains.
             */
            mainWindow.document.body.style.visibility = 'visible';
        }
    }

    function goToPage(pageNumber) {
        if (isNaN(pageNumber)) {
            pageNumber = 1;
        }

        if (pageNumber <= 0) pageNumber = 1;
        if (pageNumber >= booklet.length) pageNumber = booklet.length - 2;

        previousPage.setPage(booklet[pageNumber - 1]);
        currentPage.setPage(booklet[pageNumber]);
        nextPage.setPage(booklet[pageNumber + 1]);

        previousPage.setPosition(PREVIOUS_PAGE_POSITION);
        currentPage.setPosition(CURRENT_PAGE_POSITION);
        nextPage.setPosition(NEXT_PAGE_POSITION);

        /*
         * Set the view to the current page.
         */
        mainWindow.scrollTo(pageWidth, 0);
    }

    function paginate(pageHeight, body) {

        var contentHeight = body.scrollHeight;

        function PageContent(element, rect) {
            this.element    = element;
            this.top        = rect.top;
            this.bottom     = rect.bottom;
        }

        function sortContent(a, b) {
            return a.top - b.top;
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
        content.reverse(sortContent);

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
        var pageBottom  = ct.top + pageHeight;
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
                // debug("Element " + ct.element + " doesn't start on this page. Ending page.");
                /*
                 * Create the page and add it to the list.
                 */
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
                // debug("Element " + ct.element + " starts on the current page. Continuing on.");
                ct = content.pop();
                continue;
            }

            /*
             * This content doesn't end on this page. A new page will
             * be created. But is it too tall so it would have to be clipped
             * in two or would it fit into the next page?
             */
            if (ct.bottom - ct.top <= pageHeight) {
                /*
                 * The content would fit into the next page. No need to split.
                 * Simply create a new page.
                 */
                // debug("Element " + ct.element + " doesn't end on this page. Moving to the next.");
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
             * This content would need to be clipped. Create a new page.
             */
            // debug("Element " + ct.element + " needs to be clipped.");
            page = new PageMetrics(
                pages.length, pageStart.top,
                pageBottom, pageStart.element);
            pages.push(page);

            /*
             * Update the stuff for the next page.
             *
             * Note that:
             * 1. One needs to hack the `top` property of pageStart in
             *    order to proceed on.
             * 2. The page should have full height.
             */
            pageStart = ct;
            pageStart.top = pageBottom;
            pageBottom += pageHeight;

            /*
             * Don't pop the content. This one might need to be clipped again,
             * so simply continue.
             */
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

    function createPagesViews() {
        /*
         * We need three pages to be ready at the same time.
         */

        /*
         * First get the only page and set its id.
         */
        var np = mainWindow.document.getElementsByClassName("page")[0];
        np.id = "nextPage";
        nextPage = new PageView(np);

        /*
         * Create the previous page.
         */
        var cp = np.cloneNode(true);
        cp.id = "currentPage";
        np.parentNode.insertBefore(cp, np);
        currentPage = new PageView(cp);

        /*
         * And the last one.
         */
        var pp = cp.cloneNode(true);
        pp.id = "previousPage";
        np.parentNode.insertBefore(pp, cp);
        previousPage = new PageView(pp);
    }
}
