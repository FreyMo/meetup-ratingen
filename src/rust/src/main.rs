fn main() {
    println!("Hello, world!");
}

pub fn do_something() {
    match chrono::NaiveDate::from_ymd_opt(2025, 2, 29) {
        Some(_timestamp) => {
            // Do something with timestamp
        }
        None => {
            // It turns out that 2025 is not a leap year
        }
    };

    let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 29);

    // Provides a default value in case there is no value
    let _ = timestamp.or(Some(chrono::NaiveDate::MAX));
    let _ = timestamp.or_else(|| Some(chrono::NaiveDate::MAX));

    // Maps the NaiveDate to another one, but only if the Option is Some
    let _ = timestamp.and_then(|x| x.and_hms_opt(1, 2, 3));

    // Treats an Option as an iterator with length 0 or 1
    let _ = timestamp.filter(|x| x.leap_year());

    // Evil
    let _ = timestamp.unwrap();
}

pub fn might_be_none() -> Option<String> {
    // Returns early with None if timestamp is None
    let timestamp = chrono::NaiveDate::from_ymd_opt(2025, 2, 29)?;

    Some(timestamp.to_string())
}

pub fn might_fail() -> Result<String, chrono::ParseError> {
    // Returns early with Error if timestamp is None
    let timestamp = chrono::NaiveDate::parse_from_str("2025-02-29", "%Y-%m-%d")?;

    Ok(timestamp.to_string())
}
