/*
 * JQuery Password Test Plugin 0.2-alpha
 * https://github.com/pstepniewski/jquery-passtest
 *
 * Copyright 2013 Piotr Stepniewski
 * Released under the MIT license
 * 
 */

(function ( $ ) {

  var passtest_plugin = function(target, options) {
    
    // Class variable
    var cls = this;
    
    // Class variables
    cls.v = {};
    cls.status = {
      "len" : 0
    };

    // Default plugin settings
    cls.settings = $.extend(true, {
      "policy" : {
        "length_min"        : 0,    // Minimum password length [>=0]
        "length_max"        : 0,    // Maximum password length [>=0, 0 = option is disabled]
        "required_upper"    : 0,    // Count of required UPPERCASE characters [>=0]
        "required_lower"    : 0,    // Count of required lowercase characters [>=0]
        "required_digits"   : 0,    // Count of required numeric characters [>=0]
        "required_special"  : 0     // Count of required special characters [>=0]
      },
      "accepted"  : {
        "uppercase" : "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
        "lowercase" : "abcdefghijklmnopqrstuvwxyz",
        "numbers"   : "0123456789",
        "chars"     : "!@#$%^&*?_~();'"
      }
      ,
      // Need to be coded
      "texts": {
        "too_short" : "Your password is too short",  // Message color for hint
        "poor"      : "red",    // Message color for poor password
        "weak"      : "orange", // Message color for weak password
        "enough"    : "black",  // Message color for enough password
        "strong"    : "green"   // Message color for strong password
      }
      
    }, options );

    /*
     * Initialize plugin
     */
    
    this.initialize = function() {
      $(target).bind("change blur keyup input paste", cls.test);
      //$(target).bind("keyup", cls.test);
      cls.test();
      return;
    }

    /*
     * Password test
     */
    
    cls.test = function(nowait) {
      
      //
      // Phase 0 - Block fast-refresh calls for this method
      //
      if(nowait != true) {
        
        // Remove previously set interval
        if(cls.v.test_int) {
          clearTimeout(cls.v.test_int);
        }
        
        // Set timeout interval
        cls.v.test_int = setTimeout(function(){ cls.test(true); }, 50);
        
        // Return and re-call this method with given timeout ms
        return;
      }
      
      //
      // Phase 1 - Base test
      //
      
      // Get password score
      cls.score();
      
      // Set default status of entered password
      cls.status.ok = false;
      cls.status.message = "";

      // If no password input given (nothing to tests)
      if($(target).length == 0) {
        cls.status.message = "password_field_not_detected";
      }
      
      else {
        
        //
        // Phase 2 - Basic test
        //
      
        // Variations - check password chars consistency
        var variations = cls.variations();
  
        // If no password enteres
        if(variations.total == 0) {
          cls.status.len  = 0;
          cls.status.message = "null_password";
        }
        
        else {
          
          //
          // Phase 3 - Detailed test
          //
          
          // Password string
          str = $(target).val();
          
          // Set pass length in status
          cls.status.len = str.length;
          
          // Merge wanted characters to one string for match test
          var accepted_chars = "";
          for(var k in cls.settings.accepted) {
            accepted_chars += cls.settings.accepted[k];
          };
          // Add backslash to RegExp control characters
          accepted_chars = String(accepted_chars)
            .split("")
            .join("|")
            .replace(/\^/, "\\^")
            .replace(/\*/, "\\*")
            .replace(/\?/, "\\?");
          // Test password string for unwanted characters
          if (str.match(new RegExp("[^"+accepted_chars+"]"))) {
            cls.status.message = "unwanted_characters";
          }
          
          // Test password length (min)
          else if(cls.settings.policy.length_min > 0 && variations.total < cls.settings.policy.length_min) {
            cls.status.message = "too_short";
          }
    
          // Test password length (max)
          else if(cls.settings.policy.length_max > 0 && variations.total > cls.settings.policy.length_max) {
            cls.status.message = "too_long";
          }
          
          // Test upper chars
          else if(cls.settings.policy.required_upper > 0 && variations.upper_count < cls.settings.policy.required_upper) {
            cls.status.message = "require_uppercase";
          }
          
          // Test lower chars
          else if(cls.settings.policy.required_lower > 0 && variations.lower_count < cls.settings.policy.required_lower) {
            cls.status.message = "require_lowercase";
          }
    
          // Test digits chars
          else if(cls.settings.policy.required_digits > 0 && variations.digits_count < cls.settings.policy.required_digits) {
            cls.status.message = "require_digits";
          }
          
          // Test special chars
          else if(cls.settings.policy.required_special > 0 && variations.spec_count < cls.settings.policy.required_special) {
            cls.status.message = "require_special_characters";
          }
          
          // Test OK
          else {
            cls.status.message = "ok";
            cls.status.ok = true;
          }
        }
      }
      
      // Trigger 'passtest' event when password was tested
      $(target).trigger('passtest');
      return cls;
    }
    
    /*
     * Password variations
     */
     
    cls.variations = function() {
      
      // If no password input given (nothing to tests)
      if($(target).length == 0) {
        return 0;
      }
      
      // Password string
      var str = $(target).val();
      
      // Password characters variations
      var result = {
        "total"       : str.length,
        "chars"       : /[a-zA-Z]/.test(str),
        "chars_count" : str.replace(/[^a-zA-Z]/g, "").length,
        "digits"      : /\d/.test(str),
        "digits_count": str.replace(/[^0-9]/g, "").length,
        "lower"       : /[a-z]/.test(str),
        "lower_count" : str.replace(/[^a-z]/g, "").length,
        "upper"       : /[A-Z]/.test(str),
        "upper_count" : str.replace(/[^A-Z]/g, "").length,
        "spec"        : /\W/.test(str),
        "spec_count"  : str.replace(/\d/g, "").replace(/[A-Za-z]/g, "").length
      }
      return result;
    }
    
    /*
     * Password points
     */
     
    cls.score = function() {
      
      // Maximum score = 100 points
      
      // Variations
      var pass = cls.variations();

      // Reset counter
      var score = 0;
      
      // Score - Pass length
      if(pass.total > 0 && pass.total < 8)
        score += 30;
      
      // Score - Lower/Upper case mixing
      if(pass.upper_count != 0 && pass.lower_count != 0 && pass.total >= 8)
        score += 30;
      
      // Score - Numbers
      // -- One digit
      if(pass.digits_count >= 1 && pass.total >= 8)
        score += 30;
      
      // Score - Special characters
      // -- One char
      if (pass.spec_count >= 1 && pass.total >= 8)
          score += 30;

      if (pass.upper_count != 0 && pass.lower_count != 0 && pass.spec_count >= 1 && pass.digits_count >= 1 && pass.total >= 12) {
          score += 10;
      }
        
      // Result
      cls.status.score = score;
      return;
    }
    
  }
    
  $.fn.passtest = function(options) {
    
    /*
     * Class initialization
     */
    
    
    // Target object (password field)
    var target = this;
     
    // Plugin class
    var cls = {};
    
    // Initialize plugin on selected target
    if(!$(target).data("jquery_passtest")) {
      
      // Return new initialized plugin
      cls = new passtest_plugin(target, options);
      cls.initialize();
      $(target).data("jquery_passtest", cls);
      
    }
    
    // Get already initialized plugin on selected target
    else {
      
      // Get already initialized plugin
      cls = $(target).data("jquery_passtest");
      
      // If options given, update settings in existing plugin
      if(options) {
        cls.settings = $.extend(true, cls.settings, options);
        cls.test();
      }
      
    }
    
    // Return plugin object
    return cls;
  };

}( jQuery ));
