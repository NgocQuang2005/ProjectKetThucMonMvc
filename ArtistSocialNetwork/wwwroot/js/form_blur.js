function Validator(options) {

    var selectorRules = {};

    // thực hiện validate
    function validate(inputElement, rule) {
        var errorElement =
            inputElement.parentElement.querySelector(options.errorSelector);
        var errorMessage;
        // lấy ra các rules của các selectors
        var rules = selectorRules[rule.selector];
        // lặp qua các rules của inputElement , kiểm tra
        // nếu có lỗi thì dừng việc kiểm tra
        for (var i = 0; i < rules.length; i++) {
            errorMessage = rules[i](inputElement.value);
            if (errorMessage) {
                break;
            }
        }

        if (errorMessage) {
            errorElement.innerText = errorMessage;
            inputElement.parentElement.classList.add("invalid");
        } else {
            errorElement.innerText = "";
            inputElement.parentElement.classList.remove("invalid");
        }
    }


    // thực hiện oninput
    function oninput(inputElement) {
        var errorElement =
            inputElement.parentElement.querySelector(options.errorSelector);
        errorElement.innerText = "";
        inputElement.parentElement.classList.remove("invalid");
    }


    // lấy element của form cần validate
    var formElements = document.querySelector(options.form);
    if (formElements) {
        formElements.onsubmit = function (e) {
            e.preventDefault();
        }


        //lặp qua mỗi rule và xủ lý (lắng nghe sự kiện blur , input ...)
        options.rules.forEach(function (rule) {


            //lưu lại các rules cho mỗi input
            if (Array.isArray(selectorRules[rule.selector])) {
                selectorRules[rule.selector].push(rule.test);
            }
            else {
                selectorRules[rule.selector] = [rule.test];
            }
            // selectorRules[rule.selector] = rule.test ;



            var inputElement = formElements.querySelector(rule.selector);

            if (inputElement) {
                // xử lý trường hợp blur ra ngoài input
                inputElement.onblur = function () {
                    validate(inputElement, rule);
                };
                // xử lý trường hợp mỗi khi người dùng nhập vào input
                inputElement.oninput = function () {
                    oninput(inputElement);
                };
            }
        });
    }
}

// định nghĩa các rules
// nguyên tắc của các rules:
// 1, lỗi thì trả ra messages lỗi
// 2, kh lỗi thì không trả ra gì cả
Validator.isRequired = function (selector, messages) {
    return {
        selector: selector,
        test: function (value) {
            return value.trim() ? undefined : messages || "Vui lòng nhập trường này";
        },
    };
}; //trim() loại bỏ dấu cách ở trong input
Validator.isPhone = function (selector, messages) {
    return {
        selector: selector,
        test: function (value) {
            // Biểu thức chính quy để kiểm tra chỉ chấp nhận các số (có thể chứa khoảng trắng)
            var regex = /^\d+$/;
            return regex.test(value.trim()) ? undefined : messages || "Trường này phải là số điện thoại hợp lệ";
        },
    };
};

// các rules
Validator.isEmail = function (selector, messages) {
    return {
        selector: selector,
        test: function (value) {
            var regex = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
            return regex.test(value.trim()) ? undefined : messages || "Trường này phải là email";
        },
    };
};
Validator.isPassword = function (selector, min, messages) {
    return {
        selector: selector,
        test: function (value) {
            return value.length >= min ? undefined : messages || `vui lòng nhập tối thiểu ${min} kí tự`;
        },
    };
};

Validator.isPasswordConfirmation = function (selector, getPasswordConfim, messages) {
    return {
        selector: selector,
        test: function (value) {
            return value === getPasswordConfim() ? undefined : messages || `Giá trị nhập vào không chính xác`;
        },
    };
};