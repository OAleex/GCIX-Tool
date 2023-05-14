using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Metodo do Cabeçalho
        static void Cabecalho()
        {
            Console.WriteLine("-=-=-=-=-=-=-=- GCIX Tool -=-=-=-=-=-=-=-=-=-=-");
            Console.WriteLine("        by Alex ''OAleex'' Félix");
            Console.WriteLine("                 V1.0");
            Console.WriteLine("   Acess: oaleextraducoes.blogspot.com");
            Console.WriteLine("-=-=-=-=-=-=-=-xxxxxxxxxxx-=-=-=-=-=-=-=-=-=-=-");
        }

        // Cabeçalho + Input
        Cabecalho();
        bool inputValidado = false;
        while (!inputValidado)
        {

            Console.WriteLine("\n\nFile name:");
            string nomeDoArquivo = Console.ReadLine();

            string caminhoDoArquivo = encontrarCaminhoDoArquivo(nomeDoArquivo);
            // Condições em switch
            if (caminhoDoArquivo != null)
            {
                inputValidado = true;

                Console.WriteLine("Choices:");
                Console.WriteLine("1 - Extract datas");
                Console.WriteLine("2 - Insert datas");

                string opcao = Console.ReadLine();
                switch (opcao)
                {
                    case "1":
                        if (TemCabecalhoGCIX(caminhoDoArquivo))
                        {
                            ExtractinfoDoDump(caminhoDoArquivo);
                            Console.WriteLine("Dump successful!");
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid file.");
                            inputValidado = false;
                        }
                        break;
                    case "2":
                        if (TemCabecalhoGCIX(caminhoDoArquivo))
                        {
                            InserirInfoDoDump(caminhoDoArquivo);
                            Console.WriteLine("Insert completed!");
                        }
                        else
                        {
                            Console.WriteLine("Error: Invalid file.");
                            inputValidado = false;
                        }
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        inputValidado = false;
                        break;
                }
            }
            else
            {
                Console.WriteLine("Missing file.");
            }
        }

        Console.WriteLine("Press enter to exit.");
        Console.ReadLine();
    }


    // Função para encontrar o caminho completo de um arquivo com base no nome do mesmo
    static string encontrarCaminhoDoArquivo(string nomeDoArquivo)
    {

        // Combina o atual diretório com o nome do arquivo para obter o caminho dele completo
        string caminhoDoArquivo = Path.Combine(Directory.GetCurrentDirectory(), nomeDoArquivo);

        // Verifica se o arquivo existe no caminho completo especificado
        if (File.Exists(caminhoDoArquivo))
        {
            return caminhoDoArquivo; // Retorna o caminho completo se caso o arquivo existir
        }

        // Procura arquivos correspondentes no diretório atual com base no padrão do nome do arquivo
        string[] arquivos = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{nomeDoArquivo}.*");

        // Verifica se foram encontrados arquivos correspondentes
        if (arquivos.Length > 0)
        {
            return arquivos[0]; // Retorna o caminho do primeiro arquivo encontrado
        }

        return null; // Retorna null se nenhum arquivo correspondente for encontrado
    }

    // Esta é outra função que verificará se tem cabeçalho tipo GCIX no arquivo contêiner
    static bool TemCabecalhoGCIX(string caminhoDoArquivo)
    {
        // Lê todos os bytes do arquivo e armazena em um array
        byte[] bytesDoArquivo = File.ReadAllBytes(caminhoDoArquivo);

        // Cabeçalho que será buscado no arquivo
        string buscarStringNoArquivo = "GCIX";

        // Posição de pesquisa inicializado como 0 (padrão)
        int pesquisarIndex = 0;

        // Procura a primeira ocorrência do primeiro char do cabeçalho no array
        pesquisarIndex = Array.IndexOf(bytesDoArquivo, (byte)buscarStringNoArquivo[0], pesquisarIndex);

        // Um loop para encontrar todas as ocorrências de um cabeçalho em uma array de bytes
        while (pesquisarIndex != -1 && pesquisarIndex + buscarStringNoArquivo.Length <= bytesDoArquivo.Length)
        {
            // Pega a substring da array com base no índice de pesquisa
            string substring = System.Text.Encoding.Default.GetString(bytesDoArquivo, pesquisarIndex, buscarStringNoArquivo.Length);

            // Analisa se a substring é igual ao cabeçalho procurado
            if (substring == buscarStringNoArquivo)
                return true; // Encontrando o cabeçalho, retorna true

            // Encontra a próxima ocorrência do primeiro caractere de título na array, 
            // Iniciando a busca a partir do próximo diretório
            pesquisarIndex = Array.IndexOf(bytesDoArquivo, (byte)buscarStringNoArquivo[0], pesquisarIndex + 1);
        }

        return false; // Não encontrando o cabeçalho, retorna false
    }

    // Função para Extrair a Informação encontrada
    static void ExtractinfoDoDump(string inputDoArquivo)
    {
        // Cria uma pasta de saída com o mesmo nome do arquivo de entrada (sem a extensão)
        string pastaOutput = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputDoArquivo));
        Directory.CreateDirectory(pastaOutput);

        // Lê todos os bytes do arquivo de entrada
        byte[] bytesDoArquivo = File.ReadAllBytes(inputDoArquivo);

        // Define a string de busca no arquivo como "GCIX"
        string buscarStringNoArquivo = "GCIX";

        // Índice de pesquisa inicializado como 0 (padrão)
        int pesquisarIndex = 0;

        // Loop para encontrar e extrair as informações do dump no arquivo de entrada
        while (pesquisarIndex < bytesDoArquivo.Length)
        {
            // Encontra a próxima ocorrência da string de busca no arquivo a partir do índice de pesquisa
            pesquisarIndex = EncontrarString(bytesDoArquivo, buscarStringNoArquivo, pesquisarIndex);

            // Se não encontrar mais ocorrências, interrompe o loop
            if (pesquisarIndex == -1)
                break;

            // Define o início e o fim do índice do dump encontrado
            int inicioDoIndexDump = pesquisarIndex;
            int fimDoIndexDump = EncontrarString(bytesDoArquivo, buscarStringNoArquivo, pesquisarIndex + buscarStringNoArquivo.Length);

            // Se não encontrar o fim do índice do dump, define-o como o final do arquivo
            if (fimDoIndexDump == -1)
                fimDoIndexDump = bytesDoArquivo.Length;

            // Cria um novo array de bytes para armazenar as informações do dump
            byte[] infoDoDump = new byte[fimDoIndexDump - inicioDoIndexDump];
            Array.Copy(bytesDoArquivo, inicioDoIndexDump, infoDoDump, 0, infoDoDump.Length);

            // Define o nome do arquivo de saída com base no índice do dump encontrado
            string outputNomeDoArquivo = Path.Combine(pastaOutput, $"file_{pesquisarIndex}.dat");

            // Escreve os bytes do dump no arquivo de saída
            File.WriteAllBytes(outputNomeDoArquivo, infoDoDump);

            // Atualiza o índice de pesquisa para o próximo dump no arquivo
            pesquisarIndex = fimDoIndexDump;
        }
    }


    static void InserirInfoDoDump(string inputDoArquivo)
    {
        // Obtém o caminho da pasta onde os dumps estão armazenados
        string pastaDumpada = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(inputDoArquivo));

        // Verifica se a pasta existe
        if (Directory.Exists(pastaDumpada))
        {
            // Lê todos os bytes do arquivo de entrada
            byte[] bytesDoArquivo = File.ReadAllBytes(inputDoArquivo);

            // Percorre todos os arquivos de dump na pasta
            foreach (string arquivoDumpado in Directory.GetFiles(pastaDumpada, "file_*.dat"))
            {
                // Obtém o nome do arquivo de dump sem a extensão
                string nomeDoArquivo = Path.GetFileNameWithoutExtension(arquivoDumpado);

                // Extrai o valor decimal do índice do dump a partir do nome do arquivo
                int valorEmDecimal;
                if (int.TryParse(nomeDoArquivo.Substring(nomeDoArquivo.LastIndexOf('_') + 1), out valorEmDecimal))
                {
                    // Verifica se o valor decimal está dentro do tamanho dos bytes do arquivo original
                    if (valorEmDecimal < bytesDoArquivo.Length)
                    {
                        // Lê os bytes do arquivo de dump
                        byte[] infoDoDump = File.ReadAllBytes(arquivoDumpado);

                        // Copia os bytes do dump para o arquivo original na posição correta
                        Array.Copy(infoDoDump, 0, bytesDoArquivo, valorEmDecimal, infoDoDump.Length);
                    }
                }
            }

            // Escreve os bytes atualizados no arquivo de entrada
            File.WriteAllBytes(inputDoArquivo, bytesDoArquivo);
        }
        else
        {
            Console.WriteLine("The non-existent dumped contents folder.");
        }
    }

    // Função para encontrar a String
    static int EncontrarString(byte[] data, string buscarStringNoArquivo, int inicioDoIndex)
    {
        // Encontra a primeira ocorrência do primeiro caractere da string de busca no array de bytes,
        // a partir do índice especificado.
        int pesquisarIndex = Array.IndexOf(data, (byte)buscarStringNoArquivo[0], inicioDoIndex);

        // Entra em um loop para verificar se a substring é igual à string de busca.
        // O loop continua enquanto a posição de pesquisa não for -1 (ou seja, a string de busca não foi encontrada)
        // e o índice atual mais o comprimento da string de busca não ultrapassar o tamanho do array de bytes.
        while (pesquisarIndex != -1 && pesquisarIndex + buscarStringNoArquivo.Length <= data.Length)
        {
            // Obtém a substring do array de bytes usando a codificação padrão.
            // A substring tem o comprimento igual ao da string de busca.
            string substring = System.Text.Encoding.Default.GetString(data, pesquisarIndex, buscarStringNoArquivo.Length);

            // Verifica se a substring é igual à string de busca.
            // Se for, retorna o índice onde a string de busca foi encontrada.
            if (substring == buscarStringNoArquivo)
                return pesquisarIndex;

            // Procura a próxima ocorrência do primeiro caractere da string de busca no array de bytes,
            // a partir do próximo índice após a ocorrência atual.
            pesquisarIndex = Array.IndexOf(data, (byte)buscarStringNoArquivo[0], pesquisarIndex + 1);
        }

        // Retorna -1 se a string de busca não for encontrada no array de bytes.
        return -1;
    }
}
