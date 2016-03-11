﻿
module NakedObjects {
    import ErrorWrapper = Models.ErrorWrapper;
    import ErrorCategory = Models.ErrorCategory;
    import ClientErrorCode = Models.ClientErrorCode;

    app.provider({
        $exceptionHandler() {


            this.$get = $injector => (exception) => {
                const ulrManager: IUrlManager = $injector.get("urlManager");
                const context: IContext = $injector.get("context");

                const rp = new ErrorWrapper(ErrorCategory.ClientError, ClientErrorCode.SoftwareError, exception.message);
                rp.stackTrace = exception.stack.split("\n");

                context.setError(rp);
                ulrManager.setError(ErrorCategory.ClientError, ClientErrorCode.SoftwareError);
            };
        }
    });
}