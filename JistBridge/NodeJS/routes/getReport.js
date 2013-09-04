var netHelpers = require('../utilExports/netHelpers')
    , querystring = require('querystring')
    , fs = require('fs');

exports.doVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        var loginCreds = querystring.parse(postBuffer.utf8Data);
        fs.readFile('data/GetReport.json', 'utf8', function(err,data){
            res.setHeader('Content-Type','application/json');
            res.end(data);
        })
    });
};

exports.oldDoVerb = function (req, res) {
    netHelpers.getPostBuffer(req, function (postBuffer) {
        /*        res.end('{}');
         return;
         if ((getRandomInt(1, 4) % 3) != 2) {
         res.end('{}');
         return;
         }*/
        var report = getRandomFileFromFs();
        if(report == null || report == undefined)
            report = createRandomReport();

        var userCreds = querystring.parse(postBuffer.utf8Data);
        var response = {
            resultCode: 1,
            description: 'Success',
            report: report
        };
        /*var initOffset = getRandomInt(200, 400);
        for (var i = 0, length_i = getRandomInt(3, 9); i < length_i; ++i) {
            response.report.texts.push(getRandomOffsetAndText(initOffset));
            initOffset += getRandomInt(200, 400);
        }*/

        setTimeout(function () {
            res.setHeader('Content-Type', 'application/json');
            res.end(JSON.stringify(response));
        }, 500);
    });
};

function getRandomFileFromFs()
{
    var files = fs.readdirSync('/tmp/');

    if(files.length == 0)
        return null;

    var index = Math.floor((Math.random()*files.length));

    var inputFilename = '/tmp/' + files[index];

    if(inputFilename.indexOf('.json') == -1)
        return null;

    var fd = fs.readFileSync(inputFilename, 'utf8');
    var data =  JSON.parse(fd);

    if(data == undefined)
        return null;

    return data;
}

function createRandomReport()
{
    var report = {
        metadata: {
            resourceId: require('uid-util').create(),
                resourceField: 'reportKey',
                offsetField: 'sourceOffset',
                textField: 'label',
                fields: {
                dtg: getRandomDateAsMilDTG(),
                    //dtg: '2013-05-28T15:25:55',
                    sourceSystem: 'CIDNE',
                    analyst: 'admin'
            }
        },
        texts: [ ],
            diagram: null
    }

    var initOffset = getRandomInt(200, 400);
    for (var i = 0, length_i = getRandomInt(3, 9); i < length_i; ++i)
    {
        report.texts.push(getRandomOffsetAndText(initOffset));
        initOffset += getRandomInt(200, 400);
    }

    return report;
}



function getRandomOffsetAndText(offset) {
    return {
        offset: offset,
        text: getRandomNameA() + getRandomAction() + getRandomNameB()
    };
}

function getRandomAction() {
    var ActionList = [
        'built a wall around'
        , 'offered a gallon of milk to'
        , 'wove a basket with'
        , 'composed a sonnet about'
        , 'threw a watermelon at'
        , 'borrowed the blender of'
        , 'showed a caterpillar to'
        , 'stole a vase from'
        , 'approached a stop sign with'
        , 'performed an appendectomy on'
    ];
    return ' ' + ActionList[getRandomInt(0, ActionList.length - 1)] + ' ';
}

function getRandomDateAsMilDTG() {
    var date = getRandomDate();
    var retVal = date.getFullYear().toString() + '-';
    retVal += pad((date.getMonth() + 1).toString(), 2) + '-';
    retVal += pad((date.getDay() + 1).toString(), 2) + 'T';
    retVal += pad((date.getHours() + 1).toString(), 2) + ':';
    retVal += pad((date.getMinutes() + 1).toString(), 2) + ':';
    retVal += pad((date.getSeconds() + 1).toString(), 2);
    return retVal;
}

function getRandomDate() {
    //between 3/21/2012 & today
    var epoch = getRandomInt(1332342960000, new Date().getTime());
    return new Date(epoch);
}

function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function pad(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

function getRandomNameA() {
    var NamesList = [
        'Cynthia Stanley',
        'Iris Schacher',
        'Donald Greene',
        'Gabriel Harvell',
        'Alden Boatright',
        'Christen Carter',
        'Justin Davis',
        'Jessica Silvis',
        'Joseph Williams',
        'Nancy Johnson',
        'Josephine Zucker',
        'Molly Arrington',
        'Francis Dryden',
        'Deborah Dutton',
        'Cameron Andrews',
        'Andrea Denham',
        'Julie Jones',
        'Harland Holifield',
        'Jacqueline Woodland',
        'Charles Waldron',
        'Audrey Speaks',
        'Joy Rath',
        'Marjorie Day',
        'Peter Miller',
        'Caitlyn Corman',
        'Michael Hanes',
        'William Chaney',
        'Jean Odell',
        'Zachary Ray',
        'Luann Hunter',
        'Jason Debose',
        'Janet Gunn',
        'Steven Escamilla',
        'Derrick Mitchell',
        'Mary Green',
        'Juanita Smith',
        'Mark Curry',
        'Charles Johnson',
        'Daniel Bishop',
        'Hector Bauer',
        'Steve Miller',
        'Teresa Wightman',
        'Horace Mims',
        'Jeffrey Barker',
        'Janie Gonzalez',
        'Chris Leigh',
        'Doreen Cantu',
        'Victoria Gilbert',
        'Michael Alston',
        'Patricia Brown',
        'April Gamble',
        'Sean Hathorn',
        'Rena Wix',
        'Brandon White',
        'Bobby Goodwin',
        'Robert Johnson',
        'Justin Mattson',
        'James Garrett',
        'Ismael McLaughlin',
        'Allen Thomas',
        'Rene Patterson',
        'Albert Ware',
        'Anna Mejias',
        'Monica Thomas',
        'Michael Whitehead',
        'Carolyn Spivey',
        'Annette Pegues',
        'William Goss',
        'Troy Friedman',
        'Dominique Sanders',
        'Lillian Franck',
        'Theodore Osborne',
        'Jessica Tamayo',
        'Larry Amos',
        'Michael Gowan',
        'Frederick Timmerman',
        'Nancy Williams',
        'Doris Cole',
        'Donna Anglin',
        'William Gaunt',
        'Ruben Loza',
        'Jennifer Hensen',
        'Mark Pieper',
        'Carl Johnson',
        'James Sharkey',
        'Timothy Brasher',
        'Ronald Holmes',
        'Wayne Rice',
        'Susan Gibson',
        'Tracy Ortega',
        'Mary Foley',
        'Susan Sherrod',
        'Marlene McWhorter',
        'Tim Jones',
        'Virginia Herring',
        'Edward Jackson',
        'Marvin Willoughby',
        'Bradley King',
        'Robert Malveaux'];
    return NamesList[getRandomInt(0, NamesList.length - 1)];
}

function getRandomNameB() {
    var NamesList = [
        'Rhonda Johnson',
        'Jason Miller',
        'Adam Peters',
        'Jeff Cannon',
        'Henry Glaude',
        'Edward Mayberry',
        'Russell Dukes',
        'Rusty Roten',
        'Emanuel Holmes',
        'Gerald Parsons',
        'Harold McCormick',
        'Cindy Pike',
        'Karl Nevitt',
        'Robert Longoria',
        'Joseph Dermody',
        'Madge Hassinger',
        'Victoria Thomason',
        'Mattie Finnegan',
        'Travis Robertson',
        'Gary Wiggin',
        'Otis Schuler',
        'Gretchen Edwards',
        'Tonya Wilkinson',
        'Robert Gerst',
        'Kelly Simpson',
        'Grant Harris',
        'Robert Mejia',
        'Debra Heron',
        'Mitchell Rodriguez',
        'Cheryl Orner',
        'Joseph Koga',
        'Robert Jones',
        'Jennifer Fugate',
        'Freddie Rhinehart',
        'Joey Stamm',
        'Judy Chong',
        'Charles McCarty',
        'Chester Brauer',
        'Ronald Hopson',
        'Annie Irwin',
        'John Johnson',
        'Luis Marsh',
        'Michael Ayala',
        'Yvonne Graham',
        'Gregory Dyer',
        'Guillermo Cantrell',
        'Juan Bragg',
        'Dennis Watts',
        'Lori Vanhorn',
        'Patricia Walker',
        'Paul Owenby',
        'Vernon Rich',
        'Kelly Neff',
        'Deloris Chambers',
        'Sonya Smith',
        'Dorothy Morton',
        'Tina Nicholson',
        'Anna Ogletree',
        'Judith Barrios',
        'Linda Neese',
        'Gregory Overbay',
        'Charles Johnson',
        'Lacey Dawson',
        'Tyra Hixon',
        'Denis Edward',
        'Armando Jenkins',
        'Janice Ferrell',
        'Amber Kliebert',
        'James Perez',
        'Emily Little',
        'Anthony Ringgold',
        'Mary Jones',
        'Eric Butler',
        'Ann Slater',
        'Frances Dias',
        'Jose Pape',
        'Sam Richter',
        'Frederick Wright',
        'Elva Cromartie',
        'Karen Jackson',
        'Cassandra Ott',
        'Todd Cordell',
        'Timothy Auer',
        'Mary Allred',
        'David Souza',
        'Violet Sullivan',
        'Robert Garcia',
        'Felicia Long',
        'Edgar Begay',
        'Jaime Evans',
        'Helen Flores',
        'Wanda Davis',
        'James Jackson',
        'Betty Walls',
        'Robert Barr',
        'Kathryn Harris',
        'Israel Henderson',
        'John Neu',
        'Carol Fleming',
        'Mickey Phillips',
        'Neva Rosemond'];
    return NamesList[getRandomInt(0, NamesList.length - 1)];
}
